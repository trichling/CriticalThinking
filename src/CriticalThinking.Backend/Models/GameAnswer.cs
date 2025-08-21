namespace CriticalThinking.Backend.Models;

public class GameAnswer
{
    public int Id { get; set; }
    
    public Guid SessionId { get; set; }
    public GameSession Session { get; set; } = null!;
    
    public int FallacyId { get; set; }
    public LogicalFallacy Fallacy { get; set; } = null!;
    
    public bool IsCorrect { get; set; }
    public AnswerType AnswerType { get; set; }
}

public enum AnswerType
{
    Correct,
    Wrong,
    Missed
}