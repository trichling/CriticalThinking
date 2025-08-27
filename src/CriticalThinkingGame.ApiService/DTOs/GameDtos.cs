using CriticalThinkingGame.ApiService.Models;

namespace CriticalThinkingGame.ApiService.DTOs;

public class LogicalFallacyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
}

public class GameTextDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public List<TextFallacyDto> Fallacies { get; set; } = new();
}

public class TextFallacyDto
{
    public int Id { get; set; }
    public int LogicalFallacyId { get; set; }
    public string FallacyName { get; set; } = string.Empty;
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
}

public class StartGameRequest
{
    public string PlayerName { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public string LanguageCode { get; set; } = "en"; // Default to English
}

public class StartGameResponse
{
    public int SessionId { get; set; }
    public GameTextDto GameText { get; set; } = null!;
    public List<LogicalFallacyDto> AvailableFallacies { get; set; } = new();
}

public class SubmitAnswerRequest
{
    public int SessionId { get; set; }
    public List<int> SelectedFallacyIds { get; set; } = new();
    public string LanguageCode { get; set; } = "en"; // Default to English
}

public class SubmitAnswerResponse
{
    public int Score { get; set; }
    public int TimeTakenSeconds { get; set; }
    public List<FallacyResult> Results { get; set; } = new();
}

public class FallacyResult
{
    public int FallacyId { get; set; }
    public string FallacyName { get; set; } = string.Empty;
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public FallacyResultType ResultType { get; set; }
}

public enum FallacyResultType
{
    Correct,      // Found and selected
    Missed,       // Found but not selected
    Incorrect     // Not found but selected
}
