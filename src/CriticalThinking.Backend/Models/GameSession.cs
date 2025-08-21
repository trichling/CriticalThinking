using System.ComponentModel.DataAnnotations;

namespace CriticalThinking.Backend.Models;

public class GameSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string PlayerName { get; set; } = string.Empty;
    
    [Required]
    public Difficulty Difficulty { get; set; }
    
    public int GameTextId { get; set; }
    public GameText GameText { get; set; } = null!;
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int? TimeTakenSeconds { get; set; }
    public int? Score { get; set; }
    
    public ICollection<GameAnswer> GameAnswers { get; set; } = new List<GameAnswer>();
}