using CriticalThinkingGame.ApiService.DTOs;
using CriticalThinkingGame.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CriticalThinkingGame.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<StartGameResponse>> StartGame([FromBody] StartGameRequest request)
    {
        try
        {
            var response = await _gameService.StartGameAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("submit")]
    public async Task<ActionResult<SubmitAnswerResponse>> SubmitAnswer([FromBody] SubmitAnswerRequest request)
    {
        try
        {
            var response = await _gameService.SubmitAnswerAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("fallacies")]
    public async Task<ActionResult<List<LogicalFallacyDto>>> GetLogicalFallacies()
    {
        var fallacies = await _gameService.GetLogicalFallaciesAsync();
        return Ok(fallacies);
    }
}
