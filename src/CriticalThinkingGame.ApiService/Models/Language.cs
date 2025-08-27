using System.ComponentModel.DataAnnotations;

namespace CriticalThinkingGame.ApiService.Models;

public class Language
{
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty; // e.g., "en", "de"

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty; // e.g., "English", "Deutsch"

    [Required]
    [MaxLength(50)]
    public string NativeName { get; set; } = string.Empty; // e.g., "English", "Deutsch"

    public bool IsDefault { get; set; } = false;

    // Navigation properties
    public ICollection<LogicalFallacyTranslation> LogicalFallacyTranslations { get; set; } = new List<LogicalFallacyTranslation>();
    public ICollection<GameText> GameTexts { get; set; } = new List<GameText>();
}
