using CriticalThinking.Backend.Data;
using CriticalThinking.Backend.Models;
using CriticalThinking.Backend.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add database context
builder.AddNpgsqlDbContext<CriticalThinkingContext>(connectionName: "criticalthinking");

// Add services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IGameTextGenerationService, GameTextGenerationService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// Initialize database with sample data with retry logic
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CriticalThinkingContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    var maxAttempts = 10;
    var delay = TimeSpan.FromSeconds(2);
    
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database (attempt {Attempt}/{MaxAttempts})...", attempt, maxAttempts);
            await context.Database.EnsureCreatedAsync();

            // Seed data if database is empty
            if (!context.LogicalFallacies.Any())
            {
                logger.LogInformation("Seeding database with sample data...");
                await SeedDatabaseAsync(context);
            }

            logger.LogInformation("Database initialized successfully");
            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(ex, "Database connection failed on attempt {Attempt}/{MaxAttempts}. Retrying in {Delay} seconds...", 
                attempt, maxAttempts, delay.TotalSeconds);
            await Task.Delay(delay);
            delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 1.5, 30)); // Exponential backoff
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize database after {MaxAttempts} attempts", maxAttempts);
            throw;
        }
    }
}

static async Task SeedDatabaseAsync(CriticalThinkingContext context)
{
    // Only seed if no data exists
    if (await context.LogicalFallacies.AnyAsync() || await context.Topics.AnyAsync())
    {
        return; // Data already exists, skip seeding
    }

    // Add logical fallacies
    var fallacies = new[]
    {
        // Easy Fallacies
        new LogicalFallacy { Name = "Ad Hominem", Key = "ad-hominem", Description = "Attacking the person making the argument rather than the argument itself.", Difficulty = Difficulty.Easy, Example = "You can't trust John's opinion on climate change because he's not a scientist." },
        new LogicalFallacy { Name = "Strawman", Key = "strawman", Description = "Misrepresenting someone's argument to make it easier to attack.", Difficulty = Difficulty.Easy, Example = "Person A: We should have better funding for schools. Person B: Why do you want to waste money?" },
        new LogicalFallacy { Name = "Appeal to Authority", Key = "appeal-to-authority", Description = "Using the opinion of an authority figure, or institution of authority, in place of an actual argument.", Difficulty = Difficulty.Easy, Example = "Dr. Smith says this medicine works, so it must be true (when Dr. Smith is not a medical doctor)." },
        new LogicalFallacy { Name = "False Dilemma", Key = "false-dilemma", Description = "Presenting two alternative states as the only possibilities when more possibilities exist.", Difficulty = Difficulty.Easy, Example = "You're either with us or against us." },
        new LogicalFallacy { Name = "Slippery Slope", Key = "slippery-slope", Description = "Asserting that if we allow A to happen, then Z will consequently happen too.", Difficulty = Difficulty.Easy, Example = "If we allow students to redo this test, soon they'll want to redo every assignment." },
        new LogicalFallacy { Name = "Appeal to Emotion", Key = "appeal-to-emotion", Description = "Manipulating an emotional response in place of a valid or compelling argument.", Difficulty = Difficulty.Easy, Example = "Think of the children! We must ban this immediately." },
        new LogicalFallacy { Name = "Bandwagon", Key = "bandwagon", Description = "Appealing to popularity or the fact that many people do something as an attempted form of validation.", Difficulty = Difficulty.Easy, Example = "Everyone is buying this product, so it must be good." },
        new LogicalFallacy { Name = "Tu Quoque", Key = "tu-quoque", Description = "Avoiding having to engage with criticism by turning it back on the accuser.", Difficulty = Difficulty.Easy, Example = "How can you criticize my driving when you got a speeding ticket last year?" },

        // Medium Fallacies
        new LogicalFallacy { Name = "Burden of Proof", Key = "burden-of-proof", Description = "Claiming that the burden of proof lies not with the person making the claim, but with someone else to disprove.", Difficulty = Difficulty.Medium, Example = "I don't have to prove God exists. You have to prove he doesn't." },
        new LogicalFallacy { Name = "No True Scotsman", Key = "no-true-scotsman", Description = "Making what could be called an appeal to purity as a way to dismiss relevant criticisms.", Difficulty = Difficulty.Medium, Example = "No true Christian would support that policy." },
        new LogicalFallacy { Name = "The Texas Sharpshooter", Key = "texas-sharpshooter", Description = "Cherry-picking data clusters to suit an argument, or finding a pattern to fit a presumption.", Difficulty = Difficulty.Medium, Example = "Looking at a wall full of bullet holes and drawing a target around the tightest cluster." },
        new LogicalFallacy { Name = "Appeal to Nature", Key = "appeal-to-nature", Description = "Making the argument that because something is 'natural' it is therefore valid, justified, inevitable, or good.", Difficulty = Difficulty.Medium, Example = "This herbal remedy is natural, so it must be safe and effective." },
        new LogicalFallacy { Name = "Composition/Division", Key = "composition-division", Description = "Assuming that what's true about one part of something has to be applied to all parts of it.", Difficulty = Difficulty.Medium, Example = "Each player on this team is excellent, so the team must be excellent." },
        new LogicalFallacy { Name = "Anecdotal", Key = "anecdotal", Description = "Using personal experience or an isolated example instead of compelling evidence.", Difficulty = Difficulty.Medium, Example = "My grandfather smoked and lived to 90, so smoking isn't harmful." },
        new LogicalFallacy { Name = "Post Hoc", Key = "post-hoc", Description = "Assuming that because B comes after A, A caused B.", Difficulty = Difficulty.Medium, Example = "I wore my lucky socks and we won the game, so the socks caused our victory." },
        new LogicalFallacy { Name = "Middle Ground", Key = "middle-ground", Description = "Claiming that a compromise between two positions is always correct.", Difficulty = Difficulty.Medium, Example = "Some say the earth is round, others say it's flat, so it must be cylindrical." },

        // Hard Fallacies
        new LogicalFallacy { Name = "Begging the Question", Key = "begging-the-question", Description = "A circular argument in which the conclusion is included in the premise.", Difficulty = Difficulty.Hard, Example = "The Bible is true because it says so in the Bible." },
        new LogicalFallacy { Name = "Special Pleading", Key = "special-pleading", Description = "Moving the goalposts to create exceptions when a claim is shown to be false.", Difficulty = Difficulty.Hard, Example = "Psychic predictions work, except when tested under controlled conditions because the skeptical energy interferes." },
        new LogicalFallacy { Name = "Appeal to Ignorance", Key = "appeal-to-ignorance", Description = "Claiming that because something can't be proven false, it must be true.", Difficulty = Difficulty.Hard, Example = "No one has proven that aliens don't exist, so they must exist." },
        new LogicalFallacy { Name = "Loaded Question", Key = "loaded-question", Description = "Asking a question that has a presumption built into it so that it can't be answered without appearing guilty.", Difficulty = Difficulty.Hard, Example = "Have you stopped beating your wife?" },
        new LogicalFallacy { Name = "Ambiguity", Key = "ambiguity", Description = "Using double meanings or ambiguities of language to mislead or misrepresent the truth.", Difficulty = Difficulty.Hard, Example = "The sign said 'fine for parking here' so I thought it was okay to park." },
        new LogicalFallacy { Name = "The Gambler's Fallacy", Key = "gamblers-fallacy", Description = "Believing that 'runs' occur to statistically independent phenomena such as roulette wheel spins.", Difficulty = Difficulty.Hard, Example = "Red has come up 5 times in a row, so black is due to come up next." },
        new LogicalFallacy { Name = "Personal Incredulity", Key = "personal-incredulity", Description = "Claiming that because one finds something difficult to understand, it's therefore not true.", Difficulty = Difficulty.Hard, Example = "I can't understand how evolution works, so it must be false." },
        new LogicalFallacy { Name = "Genetic Fallacy", Key = "genetic-fallacy", Description = "Judging something as either good or bad on the basis of where it comes from, or from whom it came.", Difficulty = Difficulty.Hard, Example = "This idea came from a communist country, so it must be bad." }
    };

    context.LogicalFallacies.AddRange(fallacies);
    await context.SaveChangesAsync();

    // Add one comprehensive topic that can accommodate all fallacy types
    var topic = new Topic 
    { 
        Name = "Community Policy Debates", 
        Description = "General community discussions covering education, health, technology, environment, and development policies", 
        Difficulty = Difficulty.Easy // Will be used for all difficulty levels
    };

    context.Topics.Add(topic);
    await context.SaveChangesAsync();

    // Create text blocks - 3 examples for each of the 24 fallacies
    var textBlocks = new List<TextBlock>();

    // EASY FALLACIES (8 fallacies × 3 examples = 24 blocks)

    // Ad Hominem (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "ad-hominem").Id, 
            Content = "We can't trust Sarah Martinez's opinion on this policy because she doesn't even live in our district.", 
            Context = "Dismissing based on residency", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "ad-hominem").Id, 
            Content = "How can we take Johnson's budget proposal seriously when he filed for bankruptcy five years ago?", 
            Context = "Personal financial attack", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "ad-hominem").Id, 
            Content = "The council member advocating for this change is just a failed lawyer looking for attention.", 
            Context = "Career-based dismissal", PositionHint = "late" }
    });

    // Strawman (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "strawman").Id, 
            Content = "The mayor says we need better infrastructure, but what he really wants is to waste money on gold-plated roads.", 
            Context = "Exaggerating infrastructure needs", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "strawman").Id, 
            Content = "Supporters of renewable energy want to shut down all power plants and leave us in the dark.", 
            Context = "Misrepresenting energy transition", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "strawman").Id, 
            Content = "Those calling for police reform actually want to eliminate all law enforcement and let criminals run wild.", 
            Context = "Twisting reform proposals", PositionHint = "late" }
    });

    // Appeal to Authority (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-authority").Id, 
            Content = "Celebrity Dr. Williams endorses this health initiative, so it must be the right approach.", 
            Context = "TV personality medical advice", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-authority").Id, 
            Content = "The famous economist from TV said this policy would work, and he has millions of followers.", 
            Context = "Media expert validation", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-authority").Id, 
            Content = "My dentist says climate change isn't real, and he's a doctor so he must understand science.", 
            Context = "Wrong domain authority", PositionHint = "late" }
    });

    // False Dilemma (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "false-dilemma").Id, 
            Content = "Either we build this highway or our economy will collapse - there are no other options.", 
            Context = "Infrastructure ultimatum", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "false-dilemma").Id, 
            Content = "You're either with progress or you're against it - make your choice.", 
            Context = "Progress binary choice", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "false-dilemma").Id, 
            Content = "We must choose between safety and freedom - we can't have both.", 
            Context = "Security vs liberty", PositionHint = "late" }
    });

    // Slippery Slope (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "slippery-slope").Id, 
            Content = "If we allow food trucks on Main Street, soon we'll have carnival rides and our downtown will be a circus.", 
            Context = "Food truck regulation", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "slippery-slope").Id, 
            Content = "Start with small tax increases and before you know it, the government will take everything we own.", 
            Context = "Tax policy escalation", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "slippery-slope").Id, 
            Content = "Allow this minor regulation and soon bureaucrats will control every aspect of our lives.", 
            Context = "Regulation expansion", PositionHint = "late" }
    });

    // Appeal to Emotion (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-emotion").Id, 
            Content = "Think of the innocent children who will suffer if we don't pass this measure immediately!", 
            Context = "Children's safety appeal", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-emotion").Id, 
            Content = "Our brave veterans didn't fight for freedom just so we could give it away with this policy.", 
            Context = "Veteran sacrifice appeal", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-emotion").Id, 
            Content = "Picture your elderly parents struggling to afford medicine - we must act now!", 
            Context = "Elder care emotional plea", PositionHint = "late" }
    });

    // Bandwagon (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "bandwagon").Id, 
            Content = "Every other city in the state has adopted this policy - we need to follow suit.", 
            Context = "Regional trend following", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "bandwagon").Id, 
            Content = "All the successful communities are doing this, and everyone knows success speaks for itself.", 
            Context = "Success trend appeal", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "bandwagon").Id, 
            Content = "The majority of residents support this initiative - the people have spoken.", 
            Context = "Popular opinion validation", PositionHint = "late" }
    });

    // Tu Quoque (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "tu-quoque").Id, 
            Content = "How can you criticize our spending when your own department went over budget last year?", 
            Context = "Budget criticism deflection", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "tu-quoque").Id, 
            Content = "You're calling for transparency, but didn't you vote against the open records bill?", 
            Context = "Transparency hypocrisy", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "tu-quoque").Id, 
            Content = "Why should we listen to your environmental concerns when you drive a gas-guzzling SUV?", 
            Context = "Environmental hypocrisy", PositionHint = "late" }
    });

    // MEDIUM FALLACIES (8 fallacies × 3 examples = 24 blocks)

    // Burden of Proof (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "burden-of-proof").Id, 
            Content = "I don't need to prove this policy will work - you need to prove it won't.", 
            Context = "Policy effectiveness challenge", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "burden-of-proof").Id, 
            Content = "The health benefits are obvious - if you disagree, prove me wrong with your studies.", 
            Context = "Health claim reversal", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "burden-of-proof").Id, 
            Content = "This development will create jobs - opponents need to demonstrate it won't.", 
            Context = "Job creation assumption", PositionHint = "late" }
    });

    // No True Scotsman (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "no-true-scotsman").Id, 
            Content = "No true community leader would oppose this beneficial initiative.", 
            Context = "Leadership purity test", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "no-true-scotsman").Id, 
            Content = "Any real environmentalist would support this green energy project.", 
            Context = "Environmental purity appeal", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "no-true-scotsman").Id, 
            Content = "No genuine fiscal conservative would vote against cutting wasteful programs.", 
            Context = "Conservative identity test", PositionHint = "late" }
    });

    // Texas Sharpshooter (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "texas-sharpshooter").Id, 
            Content = "Crime dropped 15% in these three neighborhoods after we installed cameras - clearly surveillance works.", 
            Context = "Cherry-picked crime stats", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "texas-sharpshooter").Id, 
            Content = "Look at these four successful businesses that opened after our tax breaks - the policy is clearly effective.", 
            Context = "Selective business success", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "texas-sharpshooter").Id, 
            Content = "Test scores improved at five schools with new programs - this proves our education reform works.", 
            Context = "Selected education results", PositionHint = "late" }
    });

    // Appeal to Nature (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-nature").Id, 
            Content = "This organic approach to waste management is natural, so it must be the healthiest option.", 
            Context = "Natural waste solution", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-nature").Id, 
            Content = "Traditional farming methods are natural and have worked for centuries - why change now?", 
            Context = "Agricultural tradition appeal", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-nature").Id, 
            Content = "Natural immunity is always better than artificial interventions - it's how humans evolved.", 
            Context = "Natural health argument", PositionHint = "late" }
    });

    // Composition/Division (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "composition-division").Id, 
            Content = "Each department in this agency is efficient, so the entire government must be well-run.", 
            Context = "Department to government logic", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "composition-division").Id, 
            Content = "Since every member of the planning committee is qualified, their recommendations must be perfect.", 
            Context = "Individual to group quality", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "composition-division").Id, 
            Content = "This university has an excellent reputation, so every professor there must be outstanding.", 
            Context = "Institution to individual logic", PositionHint = "late" }
    });

    // Anecdotal (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "anecdotal").Id, 
            Content = "My neighbor tried this program and saved $500 a month - it clearly works for everyone.", 
            Context = "Personal savings example", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "anecdotal").Id, 
            Content = "I know three people who got sick after the water main repair - something's wrong with our water supply.", 
            Context = "Individual health concerns", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "anecdotal").Id, 
            Content = "My grandson's school improved dramatically with these changes - this policy works.", 
            Context = "Single school experience", PositionHint = "late" }
    });

    // Post Hoc (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "post-hoc").Id, 
            Content = "Traffic accidents increased after we installed those new streetlights - they're obviously causing crashes.", 
            Context = "Lighting and accident correlation", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "post-hoc").Id, 
            Content = "The downtown renovation completed and then several businesses closed - clearly the construction hurt commerce.", 
            Context = "Renovation and business closure", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "post-hoc").Id, 
            Content = "Property values rose after the mayor took office - his policies are definitely working.", 
            Context = "Leadership and market changes", PositionHint = "late" }
    });

    // Middle Ground (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "middle-ground").Id, 
            Content = "Some want no restrictions, others want total bans - the reasonable compromise is moderate regulation.", 
            Context = "Regulation compromise", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "middle-ground").Id, 
            Content = "The truth about this budget dispute lies somewhere between the opposing viewpoints.", 
            Context = "Budget disagreement middle", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "middle-ground").Id, 
            Content = "Both sides have valid points about development - let's find a balanced solution.", 
            Context = "Development compromise", PositionHint = "late" }
    });

    // HARD FALLACIES (8 fallacies × 3 examples = 24 blocks)

    // Begging the Question (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "begging-the-question").Id, 
            Content = "This policy is necessary because we need it to address the problems it's designed to solve.", 
            Context = "Circular policy justification", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "begging-the-question").Id, 
            Content = "The budget increase is justified because it provides the funding we need for essential expenses.", 
            Context = "Circular budget reasoning", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "begging-the-question").Id, 
            Content = "We know this regulation works because it effectively achieves its intended regulatory goals.", 
            Context = "Circular regulation logic", PositionHint = "late" }
    });

    // Special Pleading (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "special-pleading").Id, 
            Content = "Statistical models usually work, but they can't account for our community's unique circumstances.", 
            Context = "Statistics exception claim", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "special-pleading").Id, 
            Content = "Evidence-based policies work elsewhere, but our situation has special factors that make them invalid here.", 
            Context = "Evidence exception argument", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "special-pleading").Id, 
            Content = "Economic principles apply generally, except when dealing with our particular local market conditions.", 
            Context = "Economic exception reasoning", PositionHint = "late" }
    });

    // Appeal to Ignorance (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-ignorance").Id, 
            Content = "No one has proven this project will fail, so we should definitely proceed with construction.", 
            Context = "Project failure absence", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-ignorance").Id, 
            Content = "There's no evidence these regulations will harm businesses, therefore they must be beneficial.", 
            Context = "Regulation harm absence", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-ignorance").Id, 
            Content = "Since critics can't demonstrate negative consequences, this policy is clearly the right choice.", 
            Context = "Negative consequence absence", PositionHint = "late" }
    });

    // Loaded Question (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "loaded-question").Id, 
            Content = "When will the mayor stop ignoring the serious infrastructure problems in our community?", 
            Context = "Mayor neglect assumption", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "loaded-question").Id, 
            Content = "How long will the council continue wasting taxpayer money on these failed programs?", 
            Context = "Council waste presumption", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "loaded-question").Id, 
            Content = "Why does the planning department keep approving these destructive development projects?", 
            Context = "Destructive development assumption", PositionHint = "late" }
    });

    // Ambiguity (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "ambiguity").Id, 
            Content = "The new park regulations are fine - they're perfectly acceptable and won't cost residents anything.", 
            Context = "Fine as acceptable/penalty", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "ambiguity").Id, 
            Content = "The bank will address all community members' concerns about the new branch location.", 
            Context = "Address as location/solve", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "ambiguity").Id, 
            Content = "The city's financial position is stable - we have solid ground under our budget foundation.", 
            Context = "Stable as steady/unchanging", PositionHint = "late" }
    });

    // Gambler's Fallacy (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "gamblers-fallacy").Id, 
            Content = "We've had three failed initiatives in a row - statistically, this next one has to succeed.", 
            Context = "Success probability misconception", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "gamblers-fallacy").Id, 
            Content = "Property values have declined for four straight years - they're due to increase next year.", 
            Context = "Market correction expectation", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "gamblers-fallacy").Id, 
            Content = "Our last five budget votes were close calls - the odds favor an easy decision this time.", 
            Context = "Vote difficulty prediction", PositionHint = "late" }
    });

    // Personal Incredulity (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "personal-incredulity").Id, 
            Content = "I can't understand how this complex economic model could possibly predict local market trends accurately.", 
            Context = "Economic model complexity", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "personal-incredulity").Id, 
            Content = "The environmental impact calculations are too complicated for me to follow - they must be wrong.", 
            Context = "Environmental calculation complexity", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "personal-incredulity").Id, 
            Content = "I don't see how renewable energy could power our entire grid - the technology seems impossible.", 
            Context = "Renewable energy skepticism", PositionHint = "late" }
    });

    // Genetic Fallacy (3 examples)
    textBlocks.AddRange(new[]
    {
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "genetic-fallacy").Id, 
            Content = "This urban planning idea came from a liberal university, so it must be impractical idealism.", 
            Context = "Liberal origin dismissal", PositionHint = "early" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "genetic-fallacy").Id, 
            Content = "The efficiency proposal originated from a for-profit consulting firm - it's obviously just about money.", 
            Context = "Corporate origin suspicion", PositionHint = "middle" },
        new TextBlock { TopicId = topic.Id, FallacyId = fallacies.First(f => f.Key == "genetic-fallacy").Id, 
            Content = "Since this policy framework comes from Washington bureaucrats, it can't address local needs.", 
            Context = "Federal origin rejection", PositionHint = "late" }
    });

    context.TextBlocks.AddRange(textBlocks);
    await context.SaveChangesAsync();
}

app.Run();