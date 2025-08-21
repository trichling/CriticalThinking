using CriticalThinking.Backend.Data;
using CriticalThinking.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalThinking.Backend.Services;

public interface IGameTextGenerationService
{
    Task<GameText> GenerateGameTextAsync(Difficulty difficulty);
}

public class GameTextGenerationService : IGameTextGenerationService
{
    private readonly CriticalThinkingContext _context;
    private readonly Random _random = new();

    public GameTextGenerationService(CriticalThinkingContext context)
    {
        _context = context;
    }

    public async Task<GameText> GenerateGameTextAsync(Difficulty difficulty)
    {
        // Get appropriate topics for this difficulty level
        var availableTopics = await _context.Topics
            .Where(t => t.Difficulty <= difficulty)
            .Include(t => t.TextBlocks)
            .ThenInclude(tb => tb.Fallacy)
            .ToListAsync();

        if (!availableTopics.Any())
        {
            throw new InvalidOperationException("No topics available for the specified difficulty level");
        }

        // Randomly select a topic
        var selectedTopic = availableTopics[_random.Next(availableTopics.Count)];

        // Determine number of fallacies based on difficulty
        int targetFallacyCount = difficulty switch
        {
            Difficulty.Easy => _random.Next(3, 5),    // 3-4 fallacies
            Difficulty.Medium => _random.Next(5, 7),  // 5-6 fallacies
            Difficulty.Hard => _random.Next(7, 10),   // 7-9 fallacies
            _ => 3
        };

        // Get available fallacies for this difficulty
        var availableFallacies = await _context.LogicalFallacies
            .Where(f => f.Difficulty <= difficulty)
            .ToListAsync();

        // Randomly select fallacies
        var selectedFallacies = availableFallacies
            .OrderBy(x => _random.Next())
            .Take(targetFallacyCount)
            .ToList();

        // Get text blocks for each selected fallacy from this topic
        var selectedTextBlocks = new List<TextBlock>();
        var usedFallacyIds = new HashSet<int>();

        foreach (var fallacy in selectedFallacies)
        {
            var fallacyBlocks = selectedTopic.TextBlocks
                .Where(tb => tb.FallacyId == fallacy.Id)
                .ToList();

            if (fallacyBlocks.Any())
            {
                // Randomly select one text block for this fallacy
                var selectedBlock = fallacyBlocks[_random.Next(fallacyBlocks.Count)];
                selectedTextBlocks.Add(selectedBlock);
                usedFallacyIds.Add(fallacy.Id);
            }
        }

        // If we don't have enough text blocks, get more from the topic (but avoid duplicate fallacies)
        while (selectedTextBlocks.Count < targetFallacyCount)
        {
            var remainingBlocks = selectedTopic.TextBlocks
                .Where(tb => !usedFallacyIds.Contains(tb.FallacyId))
                .ToList();
            
            if (!remainingBlocks.Any()) break;
            
            var additionalBlock = remainingBlocks[_random.Next(remainingBlocks.Count)];
            selectedTextBlocks.Add(additionalBlock);
            usedFallacyIds.Add(additionalBlock.FallacyId);
        }

        // Generate the composite text
        var gameText = await GenerateCompositeText(selectedTopic, selectedTextBlocks);
        
        // Save the game text to database
        _context.GameTexts.Add(gameText);
        await _context.SaveChangesAsync();

        // Create fallacy mappings with positions
        var fallacyMappings = CreateFallacyMappings(gameText, selectedTextBlocks);
        _context.GameTextFallacies.AddRange(fallacyMappings);
        await _context.SaveChangesAsync();

        // Reload with mappings
        await _context.Entry(gameText)
            .Collection(gt => gt.GameTextFallacies)
            .LoadAsync();

        return gameText;
    }

    private Task<GameText> GenerateCompositeText(Topic topic, List<TextBlock> textBlocks)
    {
        // Create a narrative structure by arranging text blocks
        var arrangedBlocks = ArrangeTextBlocks(textBlocks);
        
        // Generate connecting text to make it flow naturally
        var fullText = CreateNarrativeText(topic, arrangedBlocks);

        var gameText = new GameText
        {
            TopicId = topic.Id,
            Title = GenerateTitle(topic),
            FullText = fullText,
            Difficulty = topic.Difficulty,
            TargetFallacyCount = textBlocks.Count
        };

        return Task.FromResult(gameText);
    }

    private List<TextBlock> ArrangeTextBlocks(List<TextBlock> textBlocks)
    {
        // Arrange blocks based on position hints
        var earlyBlocks = textBlocks.Where(tb => tb.PositionHint == "early").ToList();
        var middleBlocks = textBlocks.Where(tb => tb.PositionHint == "middle").ToList();
        var lateBlocks = textBlocks.Where(tb => tb.PositionHint == "late").ToList();
        var anyBlocks = textBlocks.Where(tb => tb.PositionHint == "any").ToList();

        var arranged = new List<TextBlock>();
        
        // Shuffle each category
        arranged.AddRange(earlyBlocks.OrderBy(x => _random.Next()));
        arranged.AddRange(middleBlocks.OrderBy(x => _random.Next()));
        arranged.AddRange(anyBlocks.OrderBy(x => _random.Next()));
        arranged.AddRange(lateBlocks.OrderBy(x => _random.Next()));

        return arranged;
    }

    private string CreateNarrativeText(Topic topic, List<TextBlock> arrangedBlocks)
    {
        var narrativeIntros = GetNarrativeIntros(topic);
        var transitions = GetTransitions();
        var conclusions = GetConclusions();

        var intro = narrativeIntros[_random.Next(narrativeIntros.Count)];
        var conclusion = conclusions[_random.Next(conclusions.Count)];

        var textParts = new List<string> { intro };

        for (int i = 0; i < arrangedBlocks.Count; i++)
        {
            if (i > 0)
            {
                // Add transition
                textParts.Add(transitions[_random.Next(transitions.Count)]);
            }
            textParts.Add(arrangedBlocks[i].Content);
        }

        textParts.Add(conclusion);

        return string.Join(" ", textParts);
    }

    private List<GameTextFallacy> CreateFallacyMappings(GameText gameText, List<TextBlock> textBlocks)
    {
        var mappings = new List<GameTextFallacy>();
        var fullText = gameText.FullText;
        var processedFallacyIds = new HashSet<int>();
        
        foreach (var block in textBlocks)
        {
            // Skip if we already have a mapping for this fallacy
            if (processedFallacyIds.Contains(block.FallacyId))
                continue;

            var startIndex = fullText.IndexOf(block.Content, StringComparison.OrdinalIgnoreCase);
            if (startIndex >= 0)
            {
                mappings.Add(new GameTextFallacy
                {
                    GameTextId = gameText.Id,
                    FallacyId = block.FallacyId,
                    TextPositionStart = startIndex,
                    TextPositionEnd = startIndex + block.Content.Length
                });
                processedFallacyIds.Add(block.FallacyId);
            }
        }

        return mappings;
    }

    private string GenerateTitle(Topic topic)
    {
        var titleTemplates = topic.Name switch
        {
            "School and Education Debates" => new[] { "The School Board Meeting", "Educational Policy Debate", "Funding Discussion" },
            "Health and Nutrition" => new[] { "Community Health Forum", "Wellness Workshop", "Nutrition Debate" },
            "Technology and Social Media" => new[] { "Digital Policy Meeting", "Technology in Schools", "Social Media Discussion" },
            "Environmental Policy" => new[] { "Climate Action Summit", "Environmental Policy Review", "Green Initiative Proposal" },
            "Healthcare and Medicine" => new[] { "Medical Conference", "Healthcare Policy Forum", "Treatment Options Debate" },
            "Urban Development" => new[] { "City Planning Meeting", "Development Proposal", "Urban Policy Discussion" },
            _ => new[] { "Community Discussion", "Policy Debate", "Public Forum" }
        };

        return titleTemplates[_random.Next(titleTemplates.Length)];
    }

    private List<string> GetNarrativeIntros(Topic topic)
    {
        return topic.Name switch
        {
            "School and Education Debates" => new List<string>
            {
                "At yesterday's school board meeting,",
                "During the educational policy forum,",
                "At the parent-teacher conference,"
            },
            "Health and Nutrition" => new List<string>
            {
                "At the community health fair,",
                "During the wellness workshop,",
                "At the nutrition symposium,"
            },
            "Technology and Social Media" => new List<string>
            {
                "At the digital policy meeting,",
                "During the technology in schools discussion,",
                "At the social media impact forum,"
            },
            _ => new List<string>
            {
                "At the community meeting,",
                "During the public forum,",
                "At the policy discussion,"
            }
        };
    }

    private List<string> GetTransitions()
    {
        return new List<string>
        {
            "Additionally,",
            "Furthermore,",
            "The speaker continued,",
            "In response,",
            "Moreover,",
            "The argument proceeded with",
            "The discussion then turned to",
            "Following this point,"
        };
    }

    private List<string> GetConclusions()
    {
        return new List<string>
        {
            "The meeting concluded with mixed reactions from the audience.",
            "The discussion ended without a clear consensus.",
            "The forum wrapped up with plans for further consideration.",
            "The session concluded with commitments to review the proposals.",
            "The debate ended with participants agreeing to disagree."
        };
    }
}