using CriticalThinking.Backend.Data;
using CriticalThinking.Backend.DTOs;
using CriticalThinking.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalThinking.Backend.Services;

public interface IGameService
{
    Task<GameStartResponse> StartGameAsync(GameStartRequest request);
    Task<GameSubmitResponse> SubmitGameAsync(GameSubmitRequest request);
    Task<List<FallacyOption>> GetFallaciesByDifficultyAsync(Difficulty difficulty);
}

public class GameService : IGameService
{
    private readonly CriticalThinkingContext _context;
    private readonly IScoringService _scoringService;
    private readonly IGameTextGenerationService _gameTextGenerationService;

    public GameService(CriticalThinkingContext context, IScoringService scoringService, IGameTextGenerationService gameTextGenerationService)
    {
        _context = context;
        _scoringService = scoringService;
        _gameTextGenerationService = gameTextGenerationService;
    }

    public async Task<GameStartResponse> StartGameAsync(GameStartRequest request)
    {
        // Generate a dynamic game text based on difficulty
        var gameText = await _gameTextGenerationService.GenerateGameTextAsync(request.Difficulty);
        if (gameText == null)
        {
            throw new InvalidOperationException($"No game text available for difficulty: {request.Difficulty}");
        }

        // Create game session
        var session = new GameSession
        {
            PlayerName = request.PlayerName,
            Difficulty = request.Difficulty,
            GameTextId = gameText.Id,
            StartedAt = DateTime.UtcNow
        };

        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        // Get available fallacies for the difficulty level
        var availableFallacies = await GetFallaciesByDifficultyAsync(request.Difficulty);

        return new GameStartResponse
        {
            SessionId = session.Id.ToString(),
            Text = gameText.FullText,
            AvailableFallacies = availableFallacies,
            StartedAt = session.StartedAt.ToString("O")
        };
    }

    public async Task<GameSubmitResponse> SubmitGameAsync(GameSubmitRequest request)
    {
        var session = await _context.GameSessions
            .Include(s => s.GameText)
                .ThenInclude(gt => gt.GameTextFallacies)
                .ThenInclude(gtf => gtf.Fallacy)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId);

        if (session == null)
        {
            throw new ArgumentException("Invalid session ID", nameof(request.SessionId));
        }

        if (session.CompletedAt.HasValue)
        {
            throw new InvalidOperationException("Game session already completed");
        }

        // Calculate time taken
        var completedAt = DateTime.Parse(request.CompletedAt, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        var timeTaken = (int)(completedAt - session.StartedAt).TotalSeconds;

        // Get correct fallacies for this game text
        var correctFallacies = session.GameText.GameTextFallacies
            .Select(gtf => gtf.Fallacy)
            .ToList();

        // Analyze answers
        var results = AnalyzeAnswers(request.SelectedFallacyIds, correctFallacies, session.GameText.FullText);

        // Calculate score
        var score = _scoringService.CalculateScore(
            correctAnswers: results.Count(r => r.ResultType == "correct"),
            wrongAnswers: results.Count(r => r.ResultType == "wrong"),
            missedAnswers: results.Count(r => r.ResultType == "missed"),
            timeTakenSeconds: timeTaken,
            difficulty: session.Difficulty
        );

        // Save answers and update session
        foreach (var result in results)
        {
            var answer = new GameAnswer
            {
                SessionId = session.Id,
                FallacyId = result.FallacyId,
                IsCorrect = result.ResultType == "correct",
                AnswerType = result.ResultType switch
                {
                    "correct" => AnswerType.Correct,
                    "wrong" => AnswerType.Wrong,
                    "missed" => AnswerType.Missed,
                    _ => throw new ArgumentException($"Invalid result type: {result.ResultType}")
                }
            };
            _context.GameAnswers.Add(answer);
        }

        session.CompletedAt = completedAt;
        session.TimeTakenSeconds = timeTaken;
        session.Score = score;

        await _context.SaveChangesAsync();

        var stats = new GameStats
        {
            CorrectCount = results.Count(r => r.ResultType == "correct"),
            WrongCount = results.Count(r => r.ResultType == "wrong"),
            MissedCount = results.Count(r => r.ResultType == "missed"),
            TotalFallacies = correctFallacies.Count,
            Accuracy = correctFallacies.Count > 0 ? 
                (double)results.Count(r => r.ResultType == "correct") / correctFallacies.Count : 0
        };

        return new GameSubmitResponse
        {
            Score = score,
            TimeTakenSeconds = timeTaken,
            Results = results,
            Stats = stats
        };
    }

    public async Task<List<FallacyOption>> GetFallaciesByDifficultyAsync(Difficulty difficulty)
    {
        // For easy mode, return only easy fallacies
        // For medium mode, return easy and medium fallacies
        // For hard mode, return all fallacies
        var fallacies = difficulty switch
        {
            Difficulty.Easy => await _context.LogicalFallacies
                .Where(f => f.Difficulty == Difficulty.Easy)
                .ToListAsync(),
            Difficulty.Medium => await _context.LogicalFallacies
                .Where(f => f.Difficulty == Difficulty.Easy || f.Difficulty == Difficulty.Medium)
                .ToListAsync(),
            Difficulty.Hard => await _context.LogicalFallacies
                .ToListAsync(),
            _ => throw new ArgumentException($"Invalid difficulty: {difficulty}")
        };

        return fallacies.Select(f => new FallacyOption
        {
            Id = f.Id,
            Name = f.Name,
            Key = f.Key,
            Description = f.Description
        }).ToList();
    }


    private List<FallacyResult> AnalyzeAnswers(List<int> selectedIds, List<LogicalFallacy> correctFallacies, string gameText)
    {
        var results = new List<FallacyResult>();
        var correctIds = correctFallacies.Select(f => f.Id).ToHashSet();

        // Process correct answers (selected and in correct list)
        var correctAnswers = selectedIds.Where(id => correctIds.Contains(id));
        foreach (var id in correctAnswers)
        {
            var fallacy = correctFallacies.First(f => f.Id == id);
            results.Add(new FallacyResult
            {
                FallacyId = id,
                FallacyName = fallacy.Name,
                FallacyKey = fallacy.Key,
                ResultType = "correct",
                TextReference = ExtractFallacyReference(gameText, fallacy),
                Position = FindFallacyPosition(gameText, fallacy)
            });
        }

        // Process wrong answers (selected but not in correct list)
        var wrongAnswers = selectedIds.Where(id => !correctIds.Contains(id));
        foreach (var id in wrongAnswers)
        {
            // Get fallacy info from database
            var fallacy = _context.LogicalFallacies.FirstOrDefault(f => f.Id == id);
            if (fallacy != null)
            {
                results.Add(new FallacyResult
                {
                    FallacyId = id,
                    FallacyName = fallacy.Name,
                    FallacyKey = fallacy.Key,
                    ResultType = "wrong"
                });
            }
        }

        // Process missed answers (in correct list but not selected)
        var missedAnswers = correctIds.Where(id => !selectedIds.Contains(id));
        foreach (var id in missedAnswers)
        {
            var fallacy = correctFallacies.First(f => f.Id == id);
            results.Add(new FallacyResult
            {
                FallacyId = id,
                FallacyName = fallacy.Name,
                FallacyKey = fallacy.Key,
                ResultType = "missed",
                TextReference = ExtractFallacyReference(gameText, fallacy),
                Position = FindFallacyPosition(gameText, fallacy)
            });
        }

        return results;
    }

    private string? ExtractFallacyReference(string gameText, LogicalFallacy fallacy)
    {
        // Simple implementation - in a real app, you'd have more sophisticated text matching
        // For now, return a snippet around where the fallacy might be mentioned
        var words = fallacy.Name.ToLower().Split(' ');
        var lowerText = gameText.ToLower();
        
        foreach (var word in words)
        {
            var index = lowerText.IndexOf(word);
            if (index >= 0)
            {
                var start = Math.Max(0, index - 50);
                var end = Math.Min(gameText.Length, index + word.Length + 50);
                return "..." + gameText.Substring(start, end - start) + "...";
            }
        }

        return null;
    }

    private int? FindFallacyPosition(string gameText, LogicalFallacy fallacy)
    {
        // Simple implementation - return the position of the first word of the fallacy name
        var words = fallacy.Name.ToLower().Split(' ');
        var lowerText = gameText.ToLower();
        
        foreach (var word in words)
        {
            var index = lowerText.IndexOf(word);
            if (index >= 0)
                return index;
        }

        return null;
    }
}