using System.ComponentModel.DataAnnotations;

namespace CriticalThinking.Backend.Models;

public class Topic
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public Difficulty Difficulty { get; set; }
    
    public ICollection<TextBlock> TextBlocks { get; set; } = new List<TextBlock>();
    public ICollection<GameText> GameTexts { get; set; } = new List<GameText>();
}