using System.ComponentModel.DataAnnotations;

namespace CriticalThinkingGame.ApiService.Models;

public class GameSession
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string PlayerName { get; set; } = string.Empty;

    [Required]
    public int GameTextId { get; set; }

    [Required]
    public Difficulty Difficulty { get; set; }

    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    public DateTime? EndTime { get; set; }

    public int Score { get; set; }

    public bool IsCompleted { get; set; }

    // Navigation properties
    public GameText GameText { get; set; } = null!;
    public ICollection<GameSessionFallacy> SelectedFallacies { get; set; } = new List<GameSessionFallacy>();
}
