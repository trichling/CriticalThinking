using System.ComponentModel.DataAnnotations;

namespace CriticalThinking.Backend.Models;

public class GameText
{
    public int Id { get; set; }
    
    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;
    
    [MaxLength(200)]
    public string? Title { get; set; }
    
    [Required]
    public string FullText { get; set; } = string.Empty;
    
    [Required]
    public Difficulty Difficulty { get; set; }
    
    public int TargetFallacyCount { get; set; }
    
    public ICollection<GameTextFallacy> GameTextFallacies { get; set; } = new List<GameTextFallacy>();
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}