using System.ComponentModel.DataAnnotations;

namespace CriticalThinkingGame.ApiService.Models;

public class GameSessionFallacy
{
    public int Id { get; set; }

    [Required]
    public int GameSessionId { get; set; }

    [Required]
    public int TextFallacyId { get; set; }

    public bool IsCorrect { get; set; }

    public DateTime SelectedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public GameSession GameSession { get; set; } = null!;
    public TextFallacy TextFallacy { get; set; } = null!;
}
