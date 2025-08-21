using CriticalThinking.Backend.DTOs;
using CriticalThinking.Backend.Models;
using CriticalThinking.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CriticalThinking.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameController> _logger;

    public GameController(IGameService gameService, ILogger<GameController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<ActionResult<GameStartResponse>> StartGame([FromBody] GameStartRequest request)
    {
        try
        {
            var response = await _gameService.StartGameAsync(request);
            _logger.LogInformation("Game started for player {PlayerName} with difficulty {Difficulty}",
                request.PlayerName, request.Difficulty);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to start game: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting game for player {PlayerName}", request.PlayerName);
            return StatusCode(500, "An error occurred while starting the game");
        }
    }

    [HttpPost("submit")]
    public async Task<ActionResult<GameSubmitResponse>> SubmitGame([FromBody] GameSubmitRequest request)
    {
        try
        {
            var response = await _gameService.SubmitGameAsync(request);
            _logger.LogInformation("Game submitted for session {SessionId} with score {Score}",
                request.SessionId, response.Score);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid game submission: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Invalid game submission: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting game for session {SessionId}", request.SessionId);
            return StatusCode(500, "An error occurred while submitting the game");
        }
    }

    [HttpGet("fallacies/{difficulty}")]
    public async Task<ActionResult<List<FallacyOption>>> GetFallacies(Difficulty difficulty)
    {
        try
        {
            var fallacies = await _gameService.GetFallaciesByDifficultyAsync(difficulty);
            return Ok(fallacies);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid difficulty requested: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fallacies for difficulty {Difficulty}", difficulty);
            return StatusCode(500, "An error occurred while retrieving fallacies");
        }
    }
}