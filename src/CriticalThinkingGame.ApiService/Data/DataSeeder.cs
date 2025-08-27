using CriticalThinkingGame.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalThinkingGame.ApiService.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(GameDbContext context)
    {
        if (!await context.LogicalFallacies.AnyAsync())
        {
            await SeedLogicalFallaciesAsync(context);
        }

        if (!await context.GameTexts.AnyAsync())
        {
            await SeedGameTextsAsync(context);
        }
    }

    private static async Task SeedLogicalFallaciesAsync(GameDbContext context)
    {
        var fallacies = new List<LogicalFallacy>
        {
            // Easy fallacies (8)
            new() { Name = "Ad Hominem", Description = "Attacking the person making the argument rather than the argument itself.", Difficulty = Difficulty.Easy },
            new() { Name = "Strawman", Description = "Misrepresenting someone's argument to make it easier to attack.", Difficulty = Difficulty.Easy },
            new() { Name = "Appeal to Authority", Description = "Using the opinion of an authority figure as evidence for an argument.", Difficulty = Difficulty.Easy },
            new() { Name = "False Dilemma", Description = "Presenting two alternative states as the only possibilities.", Difficulty = Difficulty.Easy },
            new() { Name = "Slippery Slope", Description = "Asserting that one event will lead to a chain of negative events.", Difficulty = Difficulty.Easy },
            new() { Name = "Circular Reasoning", Description = "The conclusion of an argument is used as a premise of that same argument.", Difficulty = Difficulty.Easy },
            new() { Name = "Hasty Generalization", Description = "Drawing a conclusion based on a small sample size.", Difficulty = Difficulty.Easy },
            new() { Name = "Red Herring", Description = "Introducing irrelevant information to divert attention from the main argument.", Difficulty = Difficulty.Easy },

            // Medium fallacies (8)
            new() { Name = "Appeal to Emotion", Description = "Manipulating emotions rather than using valid reasoning.", Difficulty = Difficulty.Medium },
            new() { Name = "Bandwagon", Description = "Appealing to popularity or the fact that many people do something.", Difficulty = Difficulty.Medium },
            new() { Name = "Tu Quoque", Description = "Avoiding criticism by turning it back on the accuser.", Difficulty = Difficulty.Medium },
            new() { Name = "Appeal to Nature", Description = "Arguing that something is good because it's natural.", Difficulty = Difficulty.Medium },
            new() { Name = "The Fallacy Fallacy", Description = "Presuming that because a claim has been poorly argued it is therefore wrong.", Difficulty = Difficulty.Medium },
            new() { Name = "Appeal to Ignorance", Description = "Claiming something is true because it hasn't been proven false.", Difficulty = Difficulty.Medium },
            new() { Name = "Composition/Division", Description = "Assuming what's true for a part is true for the whole, or vice versa.", Difficulty = Difficulty.Medium },
            new() { Name = "No True Scotsman", Description = "Making an appeal to purity as a way to dismiss relevant criticisms.", Difficulty = Difficulty.Medium },

            // Hard fallacies (8)
            new() { Name = "Loaded Question", Description = "Asking a question that has an assumption built into it.", Difficulty = Difficulty.Hard },
            new() { Name = "Begging the Question", Description = "A form of circular reasoning where the conclusion is assumed in the premise.", Difficulty = Difficulty.Hard },
            new() { Name = "Appeal to Consequences", Description = "Arguing for or against a position based solely on the consequences.", Difficulty = Difficulty.Hard },
            new() { Name = "Middle Ground", Description = "Assuming that the compromise between two positions is always correct.", Difficulty = Difficulty.Hard },
            new() { Name = "Burden of Proof", Description = "Claiming that the burden of proof lies with someone else.", Difficulty = Difficulty.Hard },
            new() { Name = "Ambiguity", Description = "Using double meanings or ambiguities to mislead or misrepresent.", Difficulty = Difficulty.Hard },
            new() { Name = "The Gambler's Fallacy", Description = "Believing that past results affect future probabilities in independent events.", Difficulty = Difficulty.Hard },
            new() { Name = "Personal Incredulity", Description = "Saying that because one finds something difficult to understand it's untrue.", Difficulty = Difficulty.Hard }
        };

        context.LogicalFallacies.AddRange(fallacies);
        await context.SaveChangesAsync();
    }

    private static async Task SeedGameTextsAsync(GameDbContext context)
    {
        var fallacies = await context.LogicalFallacies.ToListAsync();
        var gameTexts = new List<GameText>();

        // Easy difficulty text
        var easyText = new GameText
        {
            Content = "Dr. Smith claims that climate change is real, but he's just a liberal professor who wants government funding. You can't trust anything he says because he's biased. Besides, my uncle who works in construction says it's all nonsense, and he's been outside every day for 30 years. If we start regulating carbon emissions, it will destroy our economy and lead to mass unemployment, which will cause social unrest and eventually civil war.",
            Difficulty = Difficulty.Easy
        };
        gameTexts.Add(easyText);

        // Medium difficulty text
        var mediumText = new GameText
        {
            Content = "Everyone knows that organic food is better for you because it's natural. Studies show that 90% of people prefer organic products, which proves they're superior. Critics say organic farming isn't more nutritious, but they're just paid shills for Big Agriculture. Natural pesticides used in organic farming are obviously safer than synthetic ones because they come from nature. If you care about your family's health, you have no choice but to buy organic - anything else is just irresponsible parenting.",
            Difficulty = Difficulty.Medium
        };
        gameTexts.Add(mediumText);

        // Hard difficulty text
        var hardText = new GameText
        {
            Content = "Since you can't prove that telepathy doesn't exist, we must assume it's real. The scientific establishment refuses to study it seriously because they're afraid of what they might find. When researchers do find positive results, skeptics dismiss them by saying the methodology was flawed - but this just shows how closed-minded they are. The truth is probably somewhere in the middle: telepathy exists but only under certain conditions. Don't you think it's suspicious that so many people report psychic experiences? I personally find it hard to believe that consciousness could arise from mere matter, so there must be something more to the human mind.",
            Difficulty = Difficulty.Hard
        };
        gameTexts.Add(hardText);

        context.GameTexts.AddRange(gameTexts);
        await context.SaveChangesAsync();

        // Add text fallacies for easy text
        var adHominem = fallacies.First(f => f.Name == "Ad Hominem");
        var appealToAuthority = fallacies.First(f => f.Name == "Appeal to Authority");
        var slipperySlope = fallacies.First(f => f.Name == "Slippery Slope");

        var easyTextFallacies = new List<TextFallacy>
        {
            new() { GameTextId = easyText.Id, LogicalFallacyId = adHominem.Id, StartIndex = 70, EndIndex = 180 },
            new() { GameTextId = easyText.Id, LogicalFallacyId = appealToAuthority.Id, StartIndex = 250, EndIndex = 350 },
            new() { GameTextId = easyText.Id, LogicalFallacyId = slipperySlope.Id, StartIndex = 380, EndIndex = 480 }
        };

        // Add text fallacies for medium text
        var appealToNature = fallacies.First(f => f.Name == "Appeal to Nature");
        var bandwagon = fallacies.First(f => f.Name == "Bandwagon");
        var tuQuoque = fallacies.First(f => f.Name == "Tu Quoque");
        var appealToEmotion = fallacies.First(f => f.Name == "Appeal to Emotion");
        var falseDialemma = fallacies.First(f => f.Name == "False Dilemma");

        var mediumTextFallacies = new List<TextFallacy>
        {
            new() { GameTextId = mediumText.Id, LogicalFallacyId = appealToNature.Id, StartIndex = 50, EndIndex = 95 },
            new() { GameTextId = mediumText.Id, LogicalFallacyId = bandwagon.Id, StartIndex = 95, EndIndex = 170 },
            new() { GameTextId = mediumText.Id, LogicalFallacyId = tuQuoque.Id, StartIndex = 200, EndIndex = 270 },
            new() { GameTextId = mediumText.Id, LogicalFallacyId = appealToNature.Id, StartIndex = 270, EndIndex = 360 },
            new() { GameTextId = mediumText.Id, LogicalFallacyId = appealToEmotion.Id, StartIndex = 390, EndIndex = 450 },
            new() { GameTextId = mediumText.Id, LogicalFallacyId = falseDialemma.Id, StartIndex = 450, EndIndex = 520 }
        };

        // Add text fallacies for hard text
        var appealToIgnorance = fallacies.First(f => f.Name == "Appeal to Ignorance");
        var fallacyFallacy = fallacies.First(f => f.Name == "The Fallacy Fallacy");
        var middleGround = fallacies.First(f => f.Name == "Middle Ground");
        var loadedQuestion = fallacies.First(f => f.Name == "Loaded Question");
        var personalIncredulity = fallacies.First(f => f.Name == "Personal Incredulity");
        var hastyGeneralization = fallacies.First(f => f.Name == "Hasty Generalization");
        var beggingQuestion = fallacies.First(f => f.Name == "Begging the Question");
        var appealToConsequences = fallacies.First(f => f.Name == "Appeal to Consequences");
        var burdenOfProof = fallacies.First(f => f.Name == "Burden of Proof");

        var hardTextFallacies = new List<TextFallacy>
        {
            new() { GameTextId = hardText.Id, LogicalFallacyId = appealToIgnorance.Id, StartIndex = 0, EndIndex = 70 },
            new() { GameTextId = hardText.Id, LogicalFallacyId = burdenOfProof.Id, StartIndex = 70, EndIndex = 150 },
            new() { GameTextId = hardText.Id, LogicalFallacyId = fallacyFallacy.Id, StartIndex = 220, EndIndex = 320 },
            new() { GameTextId = hardText.Id, LogicalFallacyId = middleGround.Id, StartIndex = 350, EndIndex = 430 },
            new() { GameTextId = hardText.Id, LogicalFallacyId = loadedQuestion.Id, StartIndex = 500, EndIndex = 570 },
            new() { GameTextId = hardText.Id, LogicalFallacyId = hastyGeneralization.Id, StartIndex = 570, EndIndex = 620 },
            new() { GameTextId = hardText.Id, LogicalFallacyId = personalIncredulity.Id, StartIndex = 620, EndIndex = 750 },
            new() { GameTextId = hardText.Id, LogicalFallacyId = beggingQuestion.Id, StartIndex = 750, EndIndex = 800 },
            new() { GameTextId = hardText.Id, LogicalFallacyId = appealToConsequences.Id, StartIndex = 800, EndIndex = 850 }
        };

        context.TextFallacies.AddRange(easyTextFallacies);
        context.TextFallacies.AddRange(mediumTextFallacies);
        context.TextFallacies.AddRange(hardTextFallacies);
        await context.SaveChangesAsync();
    }
}
