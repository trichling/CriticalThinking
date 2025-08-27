using System.ComponentModel.DataAnnotations;

namespace CriticalThinkingGame.ApiService.Models;

public class LogicalFallacy
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public Difficulty Difficulty { get; set; }

    // Navigation properties
    public ICollection<TextFallacy> TextFallacies { get; set; } = new List<TextFallacy>();
}

public enum Difficulty
{
    Easy = 1,
    Medium = 2,
    Hard = 3
}
