using CriticalThinking.Backend.Models;
using CriticalThinking.Backend.Services;
using Xunit;

namespace CriticalThinking.Tests.Services;

public class ScoringServiceTests
{
    private readonly ScoringService _scoringService;

    public ScoringServiceTests()
    {
        _scoringService = new ScoringService();
    }

    [Fact]
    public void CalculateScore_PerfectEasyGame_ReturnsExpectedScore()
    {
        // Arrange
        var correctAnswers = 3;
        var wrongAnswers = 0;
        var missedAnswers = 0;
        var timeTakenSeconds = 60; // 1 minute
        var difficulty = Difficulty.Easy;

        // Act
        var score = _scoringService.CalculateScore(correctAnswers, wrongAnswers, missedAnswers, timeTakenSeconds, difficulty);

        // Assert
        Assert.True(score > 300); // Base score 300 + time bonus + difficulty multiplier
    }

    [Fact]
    public void CalculateScore_PerfectMediumGame_ReturnsHigherScore()
    {
        // Arrange
        var correctAnswers = 6;
        var wrongAnswers = 0;
        var missedAnswers = 0;
        var timeTakenSeconds = 180; // 3 minutes
        var difficulty = Difficulty.Medium;

        // Act
        var score = _scoringService.CalculateScore(correctAnswers, wrongAnswers, missedAnswers, timeTakenSeconds, difficulty);

        // Assert
        Assert.True(score > 600); // Base score 600 + bonus, with 1.5x multiplier
    }

    [Fact]
    public void CalculateScore_PerfectHardGame_ReturnsHighestScore()
    {
        // Arrange
        var correctAnswers = 9;
        var wrongAnswers = 0;
        var missedAnswers = 0;
        var timeTakenSeconds = 300; // 5 minutes
        var difficulty = Difficulty.Hard;

        // Act
        var score = _scoringService.CalculateScore(correctAnswers, wrongAnswers, missedAnswers, timeTakenSeconds, difficulty);

        // Assert
        Assert.True(score > 900); // Base score 900 + bonus, with 2x multiplier
    }

    [Fact]
    public void CalculateScore_WithWrongAnswers_PenalizesScore()
    {
        // Arrange
        var correctAnswers = 2;
        var wrongAnswers = 2;
        var missedAnswers = 1;
        var timeTakenSeconds = 120;
        var difficulty = Difficulty.Easy;

        // Act
        var score = _scoringService.CalculateScore(correctAnswers, wrongAnswers, missedAnswers, timeTakenSeconds, difficulty);

        // Assert
        // Base: (2 * 100) - (2 * 25) - (1 * 50) = 200 - 50 - 50 = 100
        Assert.True(score >= 100);
        Assert.True(score < 300); // Should be less than perfect score
    }

    [Fact]
    public void CalculateScore_AllWrongAnswers_ReturnsZero()
    {
        // Arrange
        var correctAnswers = 0;
        var wrongAnswers = 5;
        var missedAnswers = 3;
        var timeTakenSeconds = 300;
        var difficulty = Difficulty.Easy;

        // Act
        var score = _scoringService.CalculateScore(correctAnswers, wrongAnswers, missedAnswers, timeTakenSeconds, difficulty);

        // Assert
        Assert.Equal(0, score); // Should not go below 0
    }

    [Theory]
    [InlineData(Difficulty.Easy, 60, true)]    // Fast for easy = bonus
    [InlineData(Difficulty.Easy, 180, false)]  // Slow for easy = no bonus
    [InlineData(Difficulty.Medium, 240, true)] // Fast for medium = bonus
    [InlineData(Difficulty.Hard, 300, true)]   // Fast for hard = bonus
    public void CalculateScore_TimeBonus_WorksCorrectly(Difficulty difficulty, int timeTaken, bool shouldHaveBonus)
    {
        // Arrange
        var correctAnswers = 1;
        var wrongAnswers = 0;
        var missedAnswers = 0;

        // Act
        var fastScore = _scoringService.CalculateScore(correctAnswers, wrongAnswers, missedAnswers, timeTaken, difficulty);
        var slowScore = _scoringService.CalculateScore(correctAnswers, wrongAnswers, missedAnswers, timeTaken * 3, difficulty);

        // Assert
        if (shouldHaveBonus)
        {
            Assert.True(fastScore > slowScore, "Fast completion should have higher score than slow completion");
        }
    }
}