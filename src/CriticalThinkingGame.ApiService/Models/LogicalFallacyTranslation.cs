using System.ComponentModel.DataAnnotations;

namespace CriticalThinkingGame.ApiService.Models;

public class LogicalFallacyTranslation
{
    public int Id { get; set; }

    [Required]
    public int LogicalFallacyId { get; set; }

    [Required]
    public int LanguageId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    // Navigation properties
    public LogicalFallacy LogicalFallacy { get; set; } = null!;
    public Language Language { get; set; } = null!;
}
