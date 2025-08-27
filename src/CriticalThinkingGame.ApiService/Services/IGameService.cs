using CriticalThinkingGame.ApiService.DTOs;
using CriticalThinkingGame.ApiService.Models;

namespace CriticalThinkingGame.ApiService.Services;

public interface IGameService
{
    Task<StartGameResponse> StartGameAsync(StartGameRequest request);
    Task<SubmitAnswerResponse> SubmitAnswerAsync(SubmitAnswerRequest request);
    Task<List<LogicalFallacyDto>> GetLogicalFallaciesAsync();
}
