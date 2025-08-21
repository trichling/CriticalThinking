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
    if (await context.LogicalFallacies.AnyAsync())
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

    // Add game texts
    var gameTexts = new[]
    {
        new GameText 
        { 
            Title = "The School Funding Debate", 
            FullText = "During yesterday's town hall meeting about school funding, local businessman Tom Richardson argued against the proposed education budget increase. \"We can't trust anything Sarah Martinez says about education,\" Richardson declared, \"she doesn't even have children of her own, so what does she know about schools?\" He continued, \"Martinez claims we need more funding, but what she really wants is to waste all our taxpayer money on unnecessary luxuries.\" The crowd murmured approval when Richardson added, \"Everyone in this room who cares about fiscal responsibility opposes this budget increase. You're either with responsible spending or you're against it - there's no middle ground here.\"", 
            Difficulty = Difficulty.Easy, 
            TargetFallacyCount = 3 
        },
        new GameText 
        { 
            Title = "The Organic Food Controversy",
            FullText = "At the community health fair, wellness blogger Jenny Chen made passionate arguments for organic food. \"Think of your children's future!\" she exclaimed, \"How can you feed them food filled with dangerous chemicals?\" When questioned about the higher costs, Chen responded, \"Dr. Williams, the famous TV personality, swears by organic foods, so they must be healthier.\" She then addressed skeptics in the audience: \"Everyone who truly cares about their family's health buys organic. Either you prioritize your family's wellbeing or you don't - it's that simple.\" The crowd was moved by her emotional appeal about protecting innocent children from harm.",
            Difficulty = Difficulty.Easy, 
            TargetFallacyCount = 3 
        },
        new GameText 
        { 
            Title = "The Social Media Debate",
            FullText = "At the parent-teacher conference, concerned mother Lisa Thompson spoke against allowing social media in schools. When tech-savvy parent Mike Johnson disagreed, Thompson shot back, \"How can you defend social media when your own teenager was caught cyberbullying last year?\" She continued with passion, \"Just imagine if our precious children become addicted to their phones and lose all ability to communicate face-to-face! We must ban social media completely.\" Thompson concluded her argument by stating, \"All the parents at Roosevelt Elementary agree with me - everyone knows social media is destroying our youth.\"",
            Difficulty = Difficulty.Easy, 
            TargetFallacyCount = 3 
        },
        new GameText 
        { 
            Title = "The Climate Action Proposal",
            FullText = "City Council member David Park presented his climate action plan at Tuesday's meeting. \"Every responsible city has implemented green policies,\" Park began, \"so we must follow suit to stay relevant.\" When asked for evidence, Park replied, \"I don't need to prove climate change is real - those who deny it need to prove it's not happening.\" He shared a personal story: \"My neighbor switched to solar panels and his electricity bill disappeared completely, proving that renewable energy saves money for everyone.\" Park dismissed critics by saying, \"No true environmentalist would oppose clean energy initiatives.\" He added, \"Since carbon emissions increased after we built the new highway, clearly highways cause climate change.\" The council ultimately decided to find a reasonable middle ground between Park's comprehensive plan and opponents' preference for no action at all.",
            Difficulty = Difficulty.Medium, 
            TargetFallacyCount = 6 
        },
        new GameText 
        { 
            Title = "The Alternative Medicine Debate",
            FullText = "Dr. Rebecca Santos defended alternative medicine at the medical conference. \"Herbal remedies are completely natural,\" she argued, \"so they're obviously safer than synthetic drugs with artificial chemicals.\" When challenged about scientific evidence, Santos responded, \"Critics can't prove these treatments don't work, which shows their effectiveness.\" She cited personal experience: \"My grandmother used turmeric for arthritis and lived to 95, proving that natural remedies extend lifespan.\" Santos dismissed pharmaceutical researchers, claiming, \"No real healer would prioritize profits over natural healing methods.\" She noted, \"After our clinic started offering acupuncture, patient satisfaction increased, clearly demonstrating that acupuncture improves health outcomes.\" The conference concluded by seeking a compromise between traditional medicine and Santos's holistic approach.",
            Difficulty = Difficulty.Medium, 
            TargetFallacyCount = 6 
        },
        new GameText 
        { 
            Title = "The Urban Development Proposal",
            FullText = "At the heated city planning meeting, developer Marcus Chen presented his controversial mixed-use project. \"This development is necessary for progress,\" Chen began, \"because progress is essential for our city's growth.\" When residents expressed concerns, Chen challenged them: \"Have you stopped opposing beneficial development in our neighborhood?\" He dismissed environmental worries by stating, \"Since I cannot understand how a small development could possibly impact the entire ecosystem, these environmental claims must be false.\" Chen cited statistical support: \"Three successful developments in the past decade prove this project will succeed,\" carefully omitting five failed projects from the same period. When questioned about funding sources, Chen deflected: \"Critics can't prove my financing is problematic, therefore it must be legitimate.\" He attacked opposition leader Sarah Kim personally: \"Kim's ideas come from San Francisco's planning department, and everyone knows San Francisco policies are fundamentally flawed.\" Chen dismissed expert concerns by labeling them: \"No true urban planner would oppose smart development like this.\" He noted coincidental timing: \"Property values increased after we announced this project, proving community support for development.\" Finally, he demanded compromise: \"The planning commission should find middle ground between my complete vision and opponents' total rejection.\"",
            Difficulty = Difficulty.Hard, 
            TargetFallacyCount = 9 
        }
    };

    context.GameTexts.AddRange(gameTexts);
    await context.SaveChangesAsync();

    // Add fallacy mappings for game texts
    var fallacyMappings = new List<GameTextFallacy>();

    // Easy Game Text 1 mappings (Ad Hominem, Strawman, False Dilemma)
    var gameText1 = gameTexts[0];
    fallacyMappings.AddRange(new[]
    {
        new GameTextFallacy { GameTextId = gameText1.Id, FallacyId = fallacies.First(f => f.Key == "ad-hominem").Id },
        new GameTextFallacy { GameTextId = gameText1.Id, FallacyId = fallacies.First(f => f.Key == "strawman").Id },
        new GameTextFallacy { GameTextId = gameText1.Id, FallacyId = fallacies.First(f => f.Key == "false-dilemma").Id }
    });

    // Easy Game Text 2 mappings (Appeal to Emotion, Appeal to Authority, Bandwagon)
    var gameText2 = gameTexts[1];
    fallacyMappings.AddRange(new[]
    {
        new GameTextFallacy { GameTextId = gameText2.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-emotion").Id },
        new GameTextFallacy { GameTextId = gameText2.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-authority").Id },
        new GameTextFallacy { GameTextId = gameText2.Id, FallacyId = fallacies.First(f => f.Key == "bandwagon").Id }
    });

    // Easy Game Text 3 mappings (Tu Quoque, Slippery Slope, Bandwagon)
    var gameText3 = gameTexts[2];
    fallacyMappings.AddRange(new[]
    {
        new GameTextFallacy { GameTextId = gameText3.Id, FallacyId = fallacies.First(f => f.Key == "tu-quoque").Id },
        new GameTextFallacy { GameTextId = gameText3.Id, FallacyId = fallacies.First(f => f.Key == "slippery-slope").Id },
        new GameTextFallacy { GameTextId = gameText3.Id, FallacyId = fallacies.First(f => f.Key == "bandwagon").Id }
    });

    // Medium Game Text 1 mappings (Bandwagon, Burden of Proof, Anecdotal, No True Scotsman, Post Hoc, Middle Ground)
    var gameText4 = gameTexts[3];
    fallacyMappings.AddRange(new[]
    {
        new GameTextFallacy { GameTextId = gameText4.Id, FallacyId = fallacies.First(f => f.Key == "bandwagon").Id },
        new GameTextFallacy { GameTextId = gameText4.Id, FallacyId = fallacies.First(f => f.Key == "burden-of-proof").Id },
        new GameTextFallacy { GameTextId = gameText4.Id, FallacyId = fallacies.First(f => f.Key == "anecdotal").Id },
        new GameTextFallacy { GameTextId = gameText4.Id, FallacyId = fallacies.First(f => f.Key == "no-true-scotsman").Id },
        new GameTextFallacy { GameTextId = gameText4.Id, FallacyId = fallacies.First(f => f.Key == "post-hoc").Id },
        new GameTextFallacy { GameTextId = gameText4.Id, FallacyId = fallacies.First(f => f.Key == "middle-ground").Id }
    });

    // Medium Game Text 2 mappings (Appeal to Nature, Appeal to Ignorance, Anecdotal, No True Scotsman, Post Hoc, Middle Ground)
    var gameText5 = gameTexts[4];
    fallacyMappings.AddRange(new[]
    {
        new GameTextFallacy { GameTextId = gameText5.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-nature").Id },
        new GameTextFallacy { GameTextId = gameText5.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-ignorance").Id },
        new GameTextFallacy { GameTextId = gameText5.Id, FallacyId = fallacies.First(f => f.Key == "anecdotal").Id },
        new GameTextFallacy { GameTextId = gameText5.Id, FallacyId = fallacies.First(f => f.Key == "no-true-scotsman").Id },
        new GameTextFallacy { GameTextId = gameText5.Id, FallacyId = fallacies.First(f => f.Key == "post-hoc").Id },
        new GameTextFallacy { GameTextId = gameText5.Id, FallacyId = fallacies.First(f => f.Key == "middle-ground").Id }
    });

    // Hard Game Text 1 mappings (All 9 hard-level fallacies)
    var gameText6 = gameTexts[5];
    fallacyMappings.AddRange(new[]
    {
        new GameTextFallacy { GameTextId = gameText6.Id, FallacyId = fallacies.First(f => f.Key == "begging-the-question").Id },
        new GameTextFallacy { GameTextId = gameText6.Id, FallacyId = fallacies.First(f => f.Key == "loaded-question").Id },
        new GameTextFallacy { GameTextId = gameText6.Id, FallacyId = fallacies.First(f => f.Key == "personal-incredulity").Id },
        new GameTextFallacy { GameTextId = gameText6.Id, FallacyId = fallacies.First(f => f.Key == "texas-sharpshooter").Id },
        new GameTextFallacy { GameTextId = gameText6.Id, FallacyId = fallacies.First(f => f.Key == "appeal-to-ignorance").Id },
        new GameTextFallacy { GameTextId = gameText6.Id, FallacyId = fallacies.First(f => f.Key == "genetic-fallacy").Id },
        new GameTextFallacy { GameTextId = gameText6.Id, FallacyId = fallacies.First(f => f.Key == "no-true-scotsman").Id },
        new GameTextFallacy { GameTextId = gameText6.Id, FallacyId = fallacies.First(f => f.Key == "post-hoc").Id },
        new GameTextFallacy { GameTextId = gameText6.Id, FallacyId = fallacies.First(f => f.Key == "middle-ground").Id }
    });

    context.GameTextFallacies.AddRange(fallacyMappings);
    await context.SaveChangesAsync();
}

app.Run();