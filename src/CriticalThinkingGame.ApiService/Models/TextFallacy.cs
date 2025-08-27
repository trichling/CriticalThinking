using System.ComponentModel.DataAnnotations;

namespace CriticalThinkingGame.ApiService.Models;

public class TextFallacy
{
    public int Id { get; set; }

    [Required]
    public int GameTextId { get; set; }

    [Required]
    public int LogicalFallacyId { get; set; }

    [Required]
    public int StartIndex { get; set; }

    [Required]
    public int EndIndex { get; set; }

    // Navigation properties
    public GameText GameText { get; set; } = null!;
    public LogicalFallacy LogicalFallacy { get; set; } = null!;

    // Related game session fallacies
    public ICollection<GameSessionFallacy> GameSessionFallacies { get; set; } = new List<GameSessionFallacy>();
}
