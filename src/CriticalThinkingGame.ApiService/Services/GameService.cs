using CriticalThinkingGame.ApiService.Data;
using CriticalThinkingGame.ApiService.DTOs;
using CriticalThinkingGame.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalThinkingGame.ApiService.Services;

public class GameService : IGameService
{
    private readonly GameDbContext _context;

    public GameService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<StartGameResponse> StartGameAsync(StartGameRequest request)
    {
        // Get the language
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Code == request.LanguageCode)
            ?? await _context.Languages.FirstAsync(l => l.IsDefault);

        // Get random text for the specified difficulty and language
        var gameText = await _context.GameTexts
            .Include(gt => gt.TextFallacies)
            .ThenInclude(tf => tf.LogicalFallacy)
            .Include(gt => gt.Language)
            .Where(gt => gt.Difficulty == request.Difficulty && gt.LanguageId == language.Id)
            .OrderBy(x => Guid.NewGuid())
            .FirstOrDefaultAsync();

        if (gameText == null)
        {
            throw new InvalidOperationException($"No game text found for difficulty {request.Difficulty} and language {request.LanguageCode}");
        }

        // Create new game session
        var gameSession = new GameSession
        {
            PlayerName = request.PlayerName,
            GameTextId = gameText.Id,
            Difficulty = request.Difficulty,
            StartTime = DateTime.UtcNow,
            IsCompleted = false,
            Score = 0
        };

        _context.GameSessions.Add(gameSession);
        await _context.SaveChangesAsync();

        // Get all available fallacies based on difficulty
        var availableFallacies = await GetAvailableFallaciesForDifficulty(request.Difficulty, language.Id);

        var response = new StartGameResponse
        {
            SessionId = gameSession.Id,
            GameText = new GameTextDto
            {
                Id = gameText.Id,
                Content = gameText.Content,
                Difficulty = gameText.Difficulty,
                Fallacies = gameText.TextFallacies.Select(tf => new TextFallacyDto
                {
                    Id = tf.Id,
                    LogicalFallacyId = tf.LogicalFallacyId,
                    FallacyName = tf.LogicalFallacy.Name,
                    StartIndex = tf.StartIndex,
                    EndIndex = tf.EndIndex
                }).ToList()
            },
            AvailableFallacies = availableFallacies
        };

        return response;
    }

    public async Task<SubmitAnswerResponse> SubmitAnswerAsync(SubmitAnswerRequest request)
    {
        // Get the language
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Code == request.LanguageCode)
            ?? await _context.Languages.FirstAsync(l => l.IsDefault);

        var gameSession = await GetGameSessionAsync(request.SessionId, language.Id);
        ValidateGameSession(gameSession);

        var timeTaken = (int)(DateTime.UtcNow - gameSession.StartTime).TotalSeconds;
        var correctFallacies = gameSession.GameText.TextFallacies.ToList();
        var correctFallacyIds = correctFallacies.Select(tf => tf.LogicalFallacyId).ToHashSet();

        var results = await AnalyzeAnswersAsync(request.SelectedFallacyIds, correctFallacies, correctFallacyIds, language.Id);
        var score = CalculateScore(results, timeTaken);

        await CompleteGameSessionAsync(gameSession, score, request.SelectedFallacyIds, correctFallacies);

        return new SubmitAnswerResponse
        {
            Score = score,
            TimeTakenSeconds = timeTaken,
            Results = results
        };
    }

    private async Task<GameSession> GetGameSessionAsync(int sessionId, int? languageId = null)
    {
        var gameSession = await _context.GameSessions
            .Include(gs => gs.GameText)
            .ThenInclude(gt => gt.TextFallacies)
            .ThenInclude(tf => tf.LogicalFallacy)
            .FirstOrDefaultAsync(gs => gs.Id == sessionId);

        if (gameSession == null)
        {
            throw new InvalidOperationException("Game session not found");
        }

        return gameSession;
    }

    private static void ValidateGameSession(GameSession gameSession)
    {
        if (gameSession.IsCompleted)
        {
            throw new InvalidOperationException("Game session already completed");
        }
    }

    private async Task<List<FallacyResult>> AnalyzeAnswersAsync(
        List<int> selectedFallacyIds,
        List<TextFallacy> correctFallacies,
        HashSet<int> correctFallacyIds,
        int languageId)
    {
        var results = new List<FallacyResult>();

        // Check correct and missed answers
        foreach (var textFallacy in correctFallacies)
        {
            var resultType = selectedFallacyIds.Contains(textFallacy.LogicalFallacyId)
                ? FallacyResultType.Correct
                : FallacyResultType.Missed;

            // Get the translated name
            var translation = await _context.LogicalFallacyTranslations
                .FirstOrDefaultAsync(t => t.LogicalFallacyId == textFallacy.LogicalFallacyId && t.LanguageId == languageId);

            results.Add(new FallacyResult
            {
                FallacyId = textFallacy.LogicalFallacyId,
                FallacyName = translation?.Name ?? textFallacy.LogicalFallacy.Name,
                StartIndex = textFallacy.StartIndex,
                EndIndex = textFallacy.EndIndex,
                ResultType = resultType
            });
        }

        // Check incorrect selections
        var incorrectSelections = selectedFallacyIds.Where(id => !correctFallacyIds.Contains(id));
        foreach (var selectedFallacyId in incorrectSelections)
        {
            var fallacy = await _context.LogicalFallacies
                .Include(f => f.Translations.Where(t => t.LanguageId == languageId))
                .FirstOrDefaultAsync(f => f.Id == selectedFallacyId);

            if (fallacy != null)
            {
                results.Add(new FallacyResult
                {
                    FallacyId = selectedFallacyId,
                    FallacyName = fallacy.Translations.FirstOrDefault()?.Name ?? fallacy.Name,
                    StartIndex = 0,
                    EndIndex = 0,
                    ResultType = FallacyResultType.Incorrect
                });
            }
        }

        return results;
    }

    private static int CalculateScore(List<FallacyResult> results, int timeTaken)
    {
        var score = 0;

        foreach (var result in results)
        {
            score += result.ResultType switch
            {
                FallacyResultType.Correct => 10,
                FallacyResultType.Incorrect => -5,
                _ => 0
            };
        }

        // Apply time bonus (max 5 points for completing under 60 seconds)
        if (timeTaken < 60)
        {
            score += Math.Max(0, 5 - (timeTaken / 12));
        }

        return Math.Max(0, score);
    }

    private async Task CompleteGameSessionAsync(
        GameSession gameSession,
        int score,
        List<int> selectedFallacyIds,
        List<TextFallacy> correctFallacies)
    {
        gameSession.EndTime = DateTime.UtcNow;
        gameSession.Score = score;
        gameSession.IsCompleted = true;

        // Save selected fallacies that were correct
        var correctSelections = selectedFallacyIds
            .Join(correctFallacies,
                id => id,
                tf => tf.LogicalFallacyId,
                (id, tf) => tf);

        foreach (var textFallacy in correctSelections)
        {
            var gameSessionFallacy = new GameSessionFallacy
            {
                GameSessionId = gameSession.Id,
                TextFallacyId = textFallacy.Id,
                IsCorrect = true,
                SelectedAt = DateTime.UtcNow
            };
            _context.GameSessionFallacies.Add(gameSessionFallacy);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<LogicalFallacyDto>> GetLogicalFallaciesAsync()
    {
        var fallacies = await _context.LogicalFallacies.ToListAsync();
        return fallacies.Select(f => new LogicalFallacyDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            Difficulty = f.Difficulty
        }).ToList();
    }

    private async Task<List<LogicalFallacyDto>> GetAvailableFallaciesForDifficulty(Difficulty difficulty, int languageId)
    {
        var query = _context.LogicalFallacies
            .Include(f => f.Translations.Where(t => t.LanguageId == languageId))
            .AsQueryable();

        switch (difficulty)
        {
            case Difficulty.Easy:
                query = query.Where(f => f.Difficulty == Difficulty.Easy);
                break;
            case Difficulty.Medium:
                query = query.Where(f => f.Difficulty == Difficulty.Easy || f.Difficulty == Difficulty.Medium);
                break;
            case Difficulty.Hard:
                // Include all fallacies for hard mode
                break;
        }

        var fallacies = await query.ToListAsync();
        return fallacies.Select(f => new LogicalFallacyDto
        {
            Id = f.Id,
            Name = f.Translations.FirstOrDefault()?.Name ?? f.Name,
            Description = f.Translations.FirstOrDefault()?.Description ?? f.Description,
            Difficulty = f.Difficulty
        }).ToList();
    }
}
