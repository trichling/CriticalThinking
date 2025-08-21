using System.ComponentModel.DataAnnotations;
using CriticalThinking.Backend.Models;

namespace CriticalThinking.Backend.DTOs;

public class GameStartRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string PlayerName { get; set; } = string.Empty;
    
    [Required]
    public Difficulty Difficulty { get; set; }
}