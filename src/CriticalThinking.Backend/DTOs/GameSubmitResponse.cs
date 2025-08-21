namespace CriticalThinking.Backend.DTOs;

public class GameSubmitResponse
{
    public int Score { get; set; }
    public int TimeTakenSeconds { get; set; }
    public List<FallacyResult> Results { get; set; } = new();
    public GameStats Stats { get; set; } = new();
}

public class FallacyResult
{
    public int FallacyId { get; set; }
    public string FallacyName { get; set; } = string.Empty;
    public string FallacyKey { get; set; } = string.Empty;
    public string ResultType { get; set; } = string.Empty; // "correct", "wrong", "missed"
    public string? TextReference { get; set; } // excerpt from text showing the fallacy
    public int? Position { get; set; } // position in text where fallacy occurs
}

public class GameStats
{
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public int MissedCount { get; set; }
    public int TotalFallacies { get; set; }
    public double Accuracy { get; set; }
}