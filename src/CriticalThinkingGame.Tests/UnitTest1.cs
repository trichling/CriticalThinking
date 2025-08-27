using CriticalThinkingGame.ApiService.Data;
using CriticalThinkingGame.ApiService.Models;
using CriticalThinkingGame.ApiService.Services;
using CriticalThinkingGame.ApiService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CriticalThinkingGame.Tests;

public class GameServiceTests
{
    private GameDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new GameDbContext(options);
        SeedTestData(context);
        return context;
    }

    private static void SeedTestData(GameDbContext context)
    {
        var fallacy = new LogicalFallacy
        {
            Id = 1,
            Name = "Ad Hominem",
            Description = "Attacking the person instead of the argument",
            Difficulty = Difficulty.Easy
        };

        var gameText = new GameText
        {
            Id = 1,
            Content = "This is a test text with fallacies.",
            Difficulty = Difficulty.Easy,
            CreatedAt = DateTime.UtcNow
        };

        var textFallacy = new TextFallacy
        {
            Id = 1,
            GameTextId = 1,
            LogicalFallacyId = 1,
            StartIndex = 10,
            EndIndex = 20
        };

        context.LogicalFallacies.Add(fallacy);
        context.GameTexts.Add(gameText);
        context.TextFallacies.Add(textFallacy);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetLogicalFallacies_ReturnsAllFallacies()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new GameService(context);

        // Act
        var result = await service.GetLogicalFallaciesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Ad Hominem", result[0].Name);
    }

    [Fact]
    public async Task StartGame_CreatesGameSession()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new GameService(context);
        var request = new StartGameRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };

        // Act
        var result = await service.StartGameAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.SessionId > 0);
        Assert.Equal("Test Player", context.GameSessions.First().PlayerName);
    }

    [Fact]
    public async Task SubmitAnswer_CalculatesCorrectScore()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new GameService(context);

        // Create a game session first
        var startRequest = new StartGameRequest
        {
            PlayerName = "Test Player",
            Difficulty = Difficulty.Easy
        };
        var startResult = await service.StartGameAsync(startRequest);

        var submitRequest = new SubmitAnswerRequest
        {
            SessionId = startResult.SessionId,
            SelectedFallacyIds = new List<int> { 1 } // Correct fallacy
        };

        // Act
        var result = await service.SubmitAnswerAsync(submitRequest);

        // Assert
        Assert.True(result.Score > 0);
        Assert.Single(result.Results);
        Assert.Equal(FallacyResultType.Correct, result.Results[0].ResultType);
    }
}
