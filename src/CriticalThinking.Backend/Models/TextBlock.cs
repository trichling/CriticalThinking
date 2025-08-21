using System.ComponentModel.DataAnnotations;

namespace CriticalThinking.Backend.Models;

public class TextBlock
{
    public int Id { get; set; }
    
    public int FallacyId { get; set; }
    public LogicalFallacy Fallacy { get; set; } = null!;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Context { get; set; }
    
    [MaxLength(20)]
    public string PositionHint { get; set; } = "any";
}