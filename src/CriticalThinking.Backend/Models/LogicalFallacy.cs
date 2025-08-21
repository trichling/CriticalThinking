using System.ComponentModel.DataAnnotations;

namespace CriticalThinking.Backend.Models;

public class LogicalFallacy
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Key { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public Difficulty Difficulty { get; set; }
    
    [Required]
    public string Example { get; set; } = string.Empty;
    
    public ICollection<TextBlock> TextBlocks { get; set; } = new List<TextBlock>();
    public ICollection<GameTextFallacy> GameTextFallacies { get; set; } = new List<GameTextFallacy>();
    public ICollection<GameAnswer> GameAnswers { get; set; } = new List<GameAnswer>();
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}