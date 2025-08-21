using System.ComponentModel.DataAnnotations;

namespace CriticalThinking.Backend.DTOs;

public class GameSubmitRequest
{
    [Required]
    public Guid SessionId { get; set; }
    
    public List<int> SelectedFallacyIds { get; set; } = new();
    
    [Required]
    public string CompletedAt { get; set; } = string.Empty;
}