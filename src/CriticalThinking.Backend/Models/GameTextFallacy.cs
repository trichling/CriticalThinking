namespace CriticalThinking.Backend.Models;

public class GameTextFallacy
{
    public int Id { get; set; }
    
    public int GameTextId { get; set; }
    public GameText GameText { get; set; } = null!;
    
    public int FallacyId { get; set; }
    public LogicalFallacy Fallacy { get; set; } = null!;
    
    public int? TextPositionStart { get; set; }
    public int? TextPositionEnd { get; set; }
}