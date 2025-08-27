using System.ComponentModel.DataAnnotations;

namespace CriticalThinkingGame.ApiService.Models;

public class GameText
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Difficulty Difficulty { get; set; }

    [Required]
    public int LanguageId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Language Language { get; set; } = null!;
    public ICollection<TextFallacy> TextFallacies { get; set; } = new List<TextFallacy>();
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}
