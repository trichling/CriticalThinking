namespace CriticalThinking.Backend.DTOs;

public class GameStartResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<FallacyOption> AvailableFallacies { get; set; } = new();
    public string StartedAt { get; set; } = string.Empty;
}

public class FallacyOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}