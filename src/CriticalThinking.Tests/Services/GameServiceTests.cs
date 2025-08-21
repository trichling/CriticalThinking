using CriticalThinking.Backend.DTOs;
using CriticalThinking.Backend.Models;
using CriticalThinking.Backend.Services;
using CriticalThinking.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CriticalThinking.Tests.Services;

public class GameServiceTests : IDisposable
{
    private readonly Mock<IScoringService> _mockScoringService;
    private readonly string _databaseName;

    public GameServiceTests()
    {
        _databaseName = Guid.NewGuid().ToString();
        _mockScoringService = new Mock<IScoringService>();
        _mockScoringService.Setup(s => s.CalculateScore(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Difficulty>()))
                          .Returns(100);
    }

    private async Task<GameService> CreateGameServiceAsync()
    {
        var context = await TestDbContextFactory.CreateContextWithSampleDataAsync(_databaseName);
        return new GameService(context, _mockScoringService.Object);
    }

    [Fact]
    public async Task StartGameAsync_ValidRequest_ReturnsGameStartResponse()
    {
        // Arrange
        var gameService = await CreateGameServiceAsync();
        var request = new GameStartRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };

        // Act
        var response = await gameService.StartGameAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.True(Guid.TryParse(response.SessionId, out var sessionGuid));
        Assert.NotEqual(Guid.Empty, sessionGuid);
        Assert.NotEmpty(response.Text);
        Assert.NotEmpty(response.AvailableFallacies);
        Assert.True(response.AvailableFallacies.All(f => f.Id > 0));
    }

    [Fact]
    public async Task StartGameAsync_EasyDifficulty_ReturnsOnlyEasyFallacies()
    {
        // Arrange
        var gameService = await CreateGameServiceAsync();
        var request = new GameStartRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };

        // Act
        var response = await gameService.StartGameAsync(request);

        // Assert
        Assert.All(response.AvailableFallacies, fallacy => 
        {
            // In our test data, fallacies 1-3 are Easy
            Assert.True(fallacy.Id <= 3);
        });
    }

    [Fact]
    public async Task GetFallaciesByDifficultyAsync_EasyDifficulty_ReturnsOnlyEasyFallacies()
    {
        // Arrange
        var gameService = await CreateGameServiceAsync();

        // Act
        var fallacies = await gameService.GetFallaciesByDifficultyAsync(Difficulty.Easy);

        // Assert
        Assert.Equal(3, fallacies.Count); // Should have 3 easy fallacies from test data
        Assert.All(fallacies, f => Assert.True(f.Id <= 3)); // IDs 1-3 are easy in test data
    }

    [Fact]
    public async Task GetFallaciesByDifficultyAsync_MediumDifficulty_ReturnsEasyAndMediumFallacies()
    {
        // Arrange
        var gameService = await CreateGameServiceAsync();

        // Act
        var fallacies = await gameService.GetFallaciesByDifficultyAsync(Difficulty.Medium);

        // Assert
        Assert.Equal(4, fallacies.Count); // Should have 3 easy + 1 medium fallacies
        Assert.All(fallacies, f => Assert.True(f.Id <= 4)); // IDs 1-4 include easy and medium
    }

    [Fact]
    public async Task GetFallaciesByDifficultyAsync_HardDifficulty_ReturnsAllFallacies()
    {
        // Arrange
        var gameService = await CreateGameServiceAsync();

        // Act
        var fallacies = await gameService.GetFallaciesByDifficultyAsync(Difficulty.Hard);

        // Assert
        Assert.Equal(5, fallacies.Count); // Should have all 5 fallacies from test data
    }

    [Fact]
    public async Task SubmitGameAsync_ValidRequest_ReturnsGameSubmitResponse()
    {
        // Arrange
        var gameService = await CreateGameServiceAsync();
        
        // Start a game first
        var startRequest = new GameStartRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };
        var startResponse = await gameService.StartGameAsync(startRequest);

        var submitRequest = new GameSubmitRequest
        {
            SessionId = Guid.Parse(startResponse.SessionId),
            SelectedFallacyIds = new List<int> { 1, 2 }, // Correct fallacies for test game text
            CompletedAt = DateTime.UtcNow.ToString("O")
        };

        // Act
        var response = await gameService.SubmitGameAsync(submitRequest);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Score >= 0);
        Assert.True(response.TimeTakenSeconds > 0);
        Assert.NotEmpty(response.Results);
        Assert.NotNull(response.Stats);
        Assert.Equal(2, response.Stats.CorrectCount); // Both fallacies should be correct
        Assert.Equal(0, response.Stats.WrongCount);
        Assert.Equal(0, response.Stats.MissedCount);
    }

    [Fact]
    public async Task SubmitGameAsync_PartiallyCorrectAnswers_ReturnsCorrectAnalysis()
    {
        // Arrange
        var gameService = await CreateGameServiceAsync();
        
        var startRequest = new GameStartRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };
        var startResponse = await gameService.StartGameAsync(startRequest);

        var submitRequest = new GameSubmitRequest
        {
            SessionId = Guid.Parse(startResponse.SessionId),
            SelectedFallacyIds = new List<int> { 1, 3 }, // 1 correct, 1 wrong (3 is not in the game text)
            CompletedAt = DateTime.UtcNow.ToString("O")
        };

        // Act
        var response = await gameService.SubmitGameAsync(submitRequest);

        // Assert
        Assert.Equal(1, response.Stats.CorrectCount); // Fallacy 1 is correct
        Assert.Equal(1, response.Stats.WrongCount);   // Fallacy 3 is wrong
        Assert.Equal(1, response.Stats.MissedCount);  // Fallacy 2 is missed
        Assert.Equal(0.5, response.Stats.Accuracy);   // 1 correct out of 2 total
    }

    [Fact]
    public async Task SubmitGameAsync_InvalidSessionId_ThrowsArgumentException()
    {
        // Arrange
        var gameService = await CreateGameServiceAsync();
        var submitRequest = new GameSubmitRequest
        {
            SessionId = Guid.NewGuid(), // Non-existent session
            SelectedFallacyIds = new List<int> { 1 },
            CompletedAt = DateTime.UtcNow.ToString("O")
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => gameService.SubmitGameAsync(submitRequest));
    }

    [Fact]
    public async Task SubmitGameAsync_AlreadyCompletedSession_ThrowsInvalidOperationException()
    {
        // Arrange
        var gameService = await CreateGameServiceAsync();
        
        var startRequest = new GameStartRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };
        var startResponse = await gameService.StartGameAsync(startRequest);

        var submitRequest = new GameSubmitRequest
        {
            SessionId = Guid.Parse(startResponse.SessionId),
            SelectedFallacyIds = new List<int> { 1, 2 },
            CompletedAt = DateTime.UtcNow.ToString("O")
        };

        // Submit once
        await gameService.SubmitGameAsync(submitRequest);

        // Act & Assert - Submit again should throw
        await Assert.ThrowsAsync<InvalidOperationException>(() => gameService.SubmitGameAsync(submitRequest));
    }

    public void Dispose()
    {
        // Cleanup is handled by in-memory database disposal
    }
}