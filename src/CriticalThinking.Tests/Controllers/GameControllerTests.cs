using CriticalThinking.Backend.Controllers;
using CriticalThinking.Backend.DTOs;
using CriticalThinking.Backend.Models;
using CriticalThinking.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CriticalThinking.Tests.Controllers;

public class GameControllerTests
{
    private readonly Mock<IGameService> _mockGameService;
    private readonly Mock<ILogger<GameController>> _mockLogger;
    private readonly GameController _controller;

    public GameControllerTests()
    {
        _mockGameService = new Mock<IGameService>();
        _mockLogger = new Mock<ILogger<GameController>>();
        _controller = new GameController(_mockGameService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task StartGame_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new GameStartRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };

        var expectedResponse = new GameStartResponse
        {
            SessionId = Guid.NewGuid().ToString(),
            Text = "Test text",
            AvailableFallacies = new List<FallacyOption>
            {
                new() { Id = 1, Name = "Test Fallacy", Key = "test", Description = "Test description" }
            },
            StartedAt = DateTime.UtcNow.ToString("O")
        };

        _mockGameService.Setup(s => s.StartGameAsync(request))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.StartGame(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GameStartResponse>(okResult.Value);
        Assert.Equal(expectedResponse.SessionId, response.SessionId);
        Assert.Equal(expectedResponse.Text, response.Text);
    }

    [Fact]
    public async Task StartGame_GameServiceThrowsInvalidOperationException_ReturnsBadRequest()
    {
        // Arrange
        var request = new GameStartRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };

        _mockGameService.Setup(s => s.StartGameAsync(request))
                       .ThrowsAsync(new InvalidOperationException("No game text available"));

        // Act
        var result = await _controller.StartGame(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("No game text available", badRequestResult.Value);
    }

    [Fact]
    public async Task StartGame_GameServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new GameStartRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };

        _mockGameService.Setup(s => s.StartGameAsync(request))
                       .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.StartGame(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
        Assert.Equal("An error occurred while starting the game", statusResult.Value);
    }

    [Fact]
    public async Task SubmitGame_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new GameSubmitRequest
        {
            SessionId = Guid.NewGuid(),
            SelectedFallacyIds = new List<int> { 1, 2 },
            CompletedAt = DateTime.UtcNow.ToString("O")
        };

        var expectedResponse = new GameSubmitResponse
        {
            Score = 150,
            TimeTakenSeconds = 120,
            Results = new List<FallacyResult>
            {
                new() { FallacyId = 1, FallacyName = "Test Fallacy", FallacyKey = "test", ResultType = "correct" }
            },
            Stats = new GameStats { CorrectCount = 1, WrongCount = 0, MissedCount = 0, TotalFallacies = 1, Accuracy = 1.0 }
        };

        _mockGameService.Setup(s => s.SubmitGameAsync(request))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SubmitGame(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GameSubmitResponse>(okResult.Value);
        Assert.Equal(expectedResponse.Score, response.Score);
        Assert.Equal(expectedResponse.TimeTakenSeconds, response.TimeTakenSeconds);
    }

    [Fact]
    public async Task SubmitGame_GameServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var request = new GameSubmitRequest
        {
            SessionId = Guid.NewGuid(),
            SelectedFallacyIds = new List<int> { 1 },
            CompletedAt = DateTime.UtcNow.ToString("O")
        };

        _mockGameService.Setup(s => s.SubmitGameAsync(request))
                       .ThrowsAsync(new ArgumentException("Invalid session ID"));

        // Act
        var result = await _controller.SubmitGame(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Invalid session ID", badRequestResult.Value);
    }

    [Fact]
    public async Task SubmitGame_GameServiceThrowsInvalidOperationException_ReturnsBadRequest()
    {
        // Arrange
        var request = new GameSubmitRequest
        {
            SessionId = Guid.NewGuid(),
            SelectedFallacyIds = new List<int> { 1 },
            CompletedAt = DateTime.UtcNow.ToString("O")
        };

        _mockGameService.Setup(s => s.SubmitGameAsync(request))
                       .ThrowsAsync(new InvalidOperationException("Game session already completed"));

        // Act
        var result = await _controller.SubmitGame(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Game session already completed", badRequestResult.Value);
    }

    [Fact]
    public async Task GetFallacies_ValidDifficulty_ReturnsOkResult()
    {
        // Arrange
        var difficulty = Difficulty.Easy;
        var expectedFallacies = new List<FallacyOption>
        {
            new() { Id = 1, Name = "Test Fallacy 1", Key = "test1", Description = "Description 1" },
            new() { Id = 2, Name = "Test Fallacy 2", Key = "test2", Description = "Description 2" }
        };

        _mockGameService.Setup(s => s.GetFallaciesByDifficultyAsync(difficulty))
                       .ReturnsAsync(expectedFallacies);

        // Act
        var result = await _controller.GetFallacies(difficulty);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var fallacies = Assert.IsType<List<FallacyOption>>(okResult.Value);
        Assert.Equal(2, fallacies.Count);
        Assert.Equal(expectedFallacies[0].Name, fallacies[0].Name);
    }

    [Fact]
    public async Task GetFallacies_GameServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var difficulty = (Difficulty)999; // Invalid difficulty

        _mockGameService.Setup(s => s.GetFallaciesByDifficultyAsync(difficulty))
                       .ThrowsAsync(new ArgumentException("Invalid difficulty"));

        // Act
        var result = await _controller.GetFallacies(difficulty);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Invalid difficulty", badRequestResult.Value);
    }

    [Fact]
    public async Task GetFallacies_GameServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var difficulty = Difficulty.Easy;

        _mockGameService.Setup(s => s.GetFallaciesByDifficultyAsync(difficulty))
                       .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetFallacies(difficulty);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
        Assert.Equal("An error occurred while retrieving fallacies", statusResult.Value);
    }
}