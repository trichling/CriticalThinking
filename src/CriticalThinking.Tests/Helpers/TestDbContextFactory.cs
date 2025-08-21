using CriticalThinking.Backend.Data;
using CriticalThinking.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalThinking.Tests.Helpers;

public static class TestDbContextFactory
{
    public static CriticalThinkingContext CreateInMemoryContext(string databaseName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<CriticalThinkingContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new CriticalThinkingContext(options);
    }

    public static async Task<CriticalThinkingContext> CreateContextWithSampleDataAsync(string databaseName = "TestDb")
    {
        var context = CreateInMemoryContext(databaseName);
        await SeedTestDataAsync(context);
        return context;
    }

    private static async Task SeedTestDataAsync(CriticalThinkingContext context)
    {
        // Add sample fallacies
        var fallacies = new List<LogicalFallacy>
        {
            new() { Id = 1, Name = "Ad Hominem", Key = "ad-hominem", Description = "Attacking the person", Difficulty = Difficulty.Easy, Example = "Example 1" },
            new() { Id = 2, Name = "Strawman", Key = "strawman", Description = "Misrepresenting argument", Difficulty = Difficulty.Easy, Example = "Example 2" },
            new() { Id = 3, Name = "Appeal to Authority", Key = "appeal-to-authority", Description = "Using authority", Difficulty = Difficulty.Easy, Example = "Example 3" },
            new() { Id = 4, Name = "Burden of Proof", Key = "burden-of-proof", Description = "Shifting burden", Difficulty = Difficulty.Medium, Example = "Example 4" },
            new() { Id = 5, Name = "Begging the Question", Key = "begging-the-question", Description = "Circular argument", Difficulty = Difficulty.Hard, Example = "Example 5" }
        };

        context.LogicalFallacies.AddRange(fallacies);

        // Add sample game text
        var gameText = new GameText
        {
            Id = 1,
            Title = "Test Text",
            FullText = "This is a test text with fallacies.",
            Difficulty = Difficulty.Easy,
            TargetFallacyCount = 2
        };

        context.GameTexts.Add(gameText);

        // Add game text fallacies
        var gameTextFallacies = new List<GameTextFallacy>
        {
            new() { Id = 1, GameTextId = 1, FallacyId = 1 },
            new() { Id = 2, GameTextId = 1, FallacyId = 2 }
        };

        context.GameTextFallacies.AddRange(gameTextFallacies);

        await context.SaveChangesAsync();
    }
}