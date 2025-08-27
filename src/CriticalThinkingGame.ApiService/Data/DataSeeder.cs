using CriticalThinkingGame.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalThinkingGame.ApiService.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(GameDbContext context)
    {
        if (!await context.Languages.AnyAsync())
        {
            await SeedLanguagesAsync(context);
        }

        if (!await context.LogicalFallacies.AnyAsync())
        {
            await SeedLogicalFallaciesAsync(context);
        }

        if (!await context.GameTexts.AnyAsync())
        {
            await SeedGameTextsAsync(context);
        }

        if (!await context.LogicalFallacyTranslations.AnyAsync())
        {
            await SeedLogicalFallacyTranslationsAsync(context);
        }
    }

    private static async Task SeedLanguagesAsync(GameDbContext context)
    {
        var languages = new List<Language>
        {
            new() { Code = "en", Name = "English", NativeName = "English", IsDefault = true },
            new() { Code = "de", Name = "German", NativeName = "Deutsch", IsDefault = false }
        };

        context.Languages.AddRange(languages);
        await context.SaveChangesAsync();
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
        var languages = await context.Languages.ToListAsync();
        var englishLang = languages.First(l => l.Code == "en");
        var germanLang = languages.First(l => l.Code == "de");
        var fallacies = await context.LogicalFallacies.ToListAsync();
        var gameTexts = new List<GameText>();

        // English texts
        var easyTextEn = new GameText
        {
            Content = "Dr. Smith claims that climate change is real, but he's just a liberal professor who wants government funding. You can't trust anything he says because he's biased. Besides, my uncle who works in construction says it's all nonsense, and he's been outside every day for 30 years. If we start regulating carbon emissions, it will destroy our economy and lead to mass unemployment, which will cause social unrest and eventually civil war.",
            Difficulty = Difficulty.Easy,
            LanguageId = englishLang.Id
        };
        gameTexts.Add(easyTextEn);

        var mediumTextEn = new GameText
        {
            Content = "Everyone knows that organic food is better for you because it's natural. Studies show that 90% of people prefer organic products, which proves they're superior. Critics say organic farming isn't more nutritious, but they're just paid shills for Big Agriculture. Natural pesticides used in organic farming are obviously safer than synthetic ones because they come from nature. If you care about your family's health, you have no choice but to buy organic - anything else is just irresponsible parenting.",
            Difficulty = Difficulty.Medium,
            LanguageId = englishLang.Id
        };
        gameTexts.Add(mediumTextEn);

        var hardTextEn = new GameText
        {
            Content = "Since you can't prove that telepathy doesn't exist, we must assume it's real. The scientific establishment refuses to study it seriously because they're afraid of what they might find. When researchers do find positive results, skeptics dismiss them by saying the methodology was flawed - but this just shows how closed-minded they are. The truth is probably somewhere in the middle: telepathy exists but only under certain conditions. Don't you think it's suspicious that so many people report psychic experiences? I personally find it hard to believe that consciousness could arise from mere matter, so there must be something more to the human mind.",
            Difficulty = Difficulty.Hard,
            LanguageId = englishLang.Id
        };
        gameTexts.Add(hardTextEn);

        // German texts
        var easyTextDe = new GameText
        {
            Content = "Dr. Müller behauptet, dass der Klimawandel real ist, aber er ist nur ein linker Professor, der staatliche Förderung will. Man kann ihm nicht trauen, weil er voreingenommen ist. Außerdem sagt mein Onkel, der im Baugewerbe arbeitet, dass das alles Unsinn ist, und er war 30 Jahre lang jeden Tag draußen. Wenn wir anfangen, Kohlenstoffemissionen zu regulieren, wird das unsere Wirtschaft zerstören und zu Massenarbeitslosigkeit führen, was soziale Unruhen und schließlich einen Bürgerkrieg verursachen wird.",
            Difficulty = Difficulty.Easy,
            LanguageId = germanLang.Id
        };
        gameTexts.Add(easyTextDe);

        var mediumTextDe = new GameText
        {
            Content = "Jeder weiß, dass Bio-Lebensmittel besser für einen sind, weil sie natürlich sind. Studien zeigen, dass 90% der Menschen Bio-Produkte bevorzugen, was beweist, dass sie überlegen sind. Kritiker sagen, dass Bio-Landwirtschaft nicht nahrhafter ist, aber das sind nur bezahlte Handlanger der Agrarindustrie. Natürliche Pestizide, die in der Bio-Landwirtschaft verwendet werden, sind offensichtlich sicherer als synthetische, weil sie aus der Natur kommen. Wenn dir die Gesundheit deiner Familie wichtig ist, hast du keine andere Wahl, als Bio zu kaufen - alles andere ist unverantwortliche Elternschaft.",
            Difficulty = Difficulty.Medium,
            LanguageId = germanLang.Id
        };
        gameTexts.Add(mediumTextDe);

        var hardTextDe = new GameText
        {
            Content = "Da du nicht beweisen kannst, dass Telepathie nicht existiert, müssen wir annehmen, dass sie real ist. Das wissenschaftliche Establishment weigert sich, sie ernsthaft zu studieren, weil sie Angst vor dem haben, was sie finden könnten. Wenn Forscher positive Ergebnisse finden, weisen Skeptiker sie ab und sagen, die Methodik sei fehlerhaft - aber das zeigt nur, wie engstirnig sie sind. Die Wahrheit liegt wahrscheinlich irgendwo in der Mitte: Telepathie existiert, aber nur unter bestimmten Bedingungen. Findest du es nicht verdächtig, dass so viele Menschen psychische Erfahrungen berichten? Ich persönlich finde es schwer zu glauben, dass Bewusstsein aus bloßer Materie entstehen könnte, also muss es etwas mehr am menschlichen Geist geben.",
            Difficulty = Difficulty.Hard,
            LanguageId = germanLang.Id
        };
        gameTexts.Add(hardTextDe);

        context.GameTexts.AddRange(gameTexts);
        await context.SaveChangesAsync();

        // Add text fallacies for English easy text
        var adHominem = fallacies.First(f => f.Name == "Ad Hominem");
        var appealToAuthority = fallacies.First(f => f.Name == "Appeal to Authority");
        var slipperySlope = fallacies.First(f => f.Name == "Slippery Slope");

        var easyTextEnFallacies = new List<TextFallacy>
        {
            new() { GameTextId = easyTextEn.Id, LogicalFallacyId = adHominem.Id, StartIndex = 70, EndIndex = 180 },
            new() { GameTextId = easyTextEn.Id, LogicalFallacyId = appealToAuthority.Id, StartIndex = 250, EndIndex = 350 },
            new() { GameTextId = easyTextEn.Id, LogicalFallacyId = slipperySlope.Id, StartIndex = 380, EndIndex = 480 }
        };

        // Add text fallacies for English medium text
        var appealToNature = fallacies.First(f => f.Name == "Appeal to Nature");
        var bandwagon = fallacies.First(f => f.Name == "Bandwagon");
        var tuQuoque = fallacies.First(f => f.Name == "Tu Quoque");
        var appealToEmotion = fallacies.First(f => f.Name == "Appeal to Emotion");
        var falseDialemma = fallacies.First(f => f.Name == "False Dilemma");

        var mediumTextEnFallacies = new List<TextFallacy>
        {
            new() { GameTextId = mediumTextEn.Id, LogicalFallacyId = appealToNature.Id, StartIndex = 50, EndIndex = 95 },
            new() { GameTextId = mediumTextEn.Id, LogicalFallacyId = bandwagon.Id, StartIndex = 95, EndIndex = 170 },
            new() { GameTextId = mediumTextEn.Id, LogicalFallacyId = tuQuoque.Id, StartIndex = 200, EndIndex = 270 },
            new() { GameTextId = mediumTextEn.Id, LogicalFallacyId = appealToNature.Id, StartIndex = 270, EndIndex = 360 },
            new() { GameTextId = mediumTextEn.Id, LogicalFallacyId = appealToEmotion.Id, StartIndex = 390, EndIndex = 450 },
            new() { GameTextId = mediumTextEn.Id, LogicalFallacyId = falseDialemma.Id, StartIndex = 450, EndIndex = 520 }
        };

        // Add text fallacies for English hard text
        var appealToIgnorance = fallacies.First(f => f.Name == "Appeal to Ignorance");
        var fallacyFallacy = fallacies.First(f => f.Name == "The Fallacy Fallacy");
        var middleGround = fallacies.First(f => f.Name == "Middle Ground");
        var loadedQuestion = fallacies.First(f => f.Name == "Loaded Question");
        var personalIncredulity = fallacies.First(f => f.Name == "Personal Incredulity");
        var hastyGeneralization = fallacies.First(f => f.Name == "Hasty Generalization");

        var hardTextEnFallacies = new List<TextFallacy>
        {
            new() { GameTextId = hardTextEn.Id, LogicalFallacyId = appealToIgnorance.Id, StartIndex = 0, EndIndex = 70 },
            new() { GameTextId = hardTextEn.Id, LogicalFallacyId = fallacyFallacy.Id, StartIndex = 220, EndIndex = 320 },
            new() { GameTextId = hardTextEn.Id, LogicalFallacyId = middleGround.Id, StartIndex = 350, EndIndex = 430 },
            new() { GameTextId = hardTextEn.Id, LogicalFallacyId = loadedQuestion.Id, StartIndex = 500, EndIndex = 570 },
            new() { GameTextId = hardTextEn.Id, LogicalFallacyId = hastyGeneralization.Id, StartIndex = 570, EndIndex = 620 },
            new() { GameTextId = hardTextEn.Id, LogicalFallacyId = personalIncredulity.Id, StartIndex = 620, EndIndex = 750 }
        };

        // Add text fallacies for German texts (with approximate positions for German text)
        var easyTextDeFallacies = new List<TextFallacy>
        {
            new() { GameTextId = easyTextDe.Id, LogicalFallacyId = adHominem.Id, StartIndex = 75, EndIndex = 160 },
            new() { GameTextId = easyTextDe.Id, LogicalFallacyId = appealToAuthority.Id, StartIndex = 200, EndIndex = 310 },
            new() { GameTextId = easyTextDe.Id, LogicalFallacyId = slipperySlope.Id, StartIndex = 340, EndIndex = 500 }
        };

        var mediumTextDeFallacies = new List<TextFallacy>
        {
            new() { GameTextId = mediumTextDe.Id, LogicalFallacyId = appealToNature.Id, StartIndex = 40, EndIndex = 85 },
            new() { GameTextId = mediumTextDe.Id, LogicalFallacyId = bandwagon.Id, StartIndex = 85, EndIndex = 160 },
            new() { GameTextId = mediumTextDe.Id, LogicalFallacyId = tuQuoque.Id, StartIndex = 190, EndIndex = 280 },
            new() { GameTextId = mediumTextDe.Id, LogicalFallacyId = appealToNature.Id, StartIndex = 280, EndIndex = 380 },
            new() { GameTextId = mediumTextDe.Id, LogicalFallacyId = appealToEmotion.Id, StartIndex = 410, EndIndex = 470 },
            new() { GameTextId = mediumTextDe.Id, LogicalFallacyId = falseDialemma.Id, StartIndex = 470, EndIndex = 550 }
        };

        var hardTextDeFallacies = new List<TextFallacy>
        {
            new() { GameTextId = hardTextDe.Id, LogicalFallacyId = appealToIgnorance.Id, StartIndex = 0, EndIndex = 80 },
            new() { GameTextId = hardTextDe.Id, LogicalFallacyId = fallacyFallacy.Id, StartIndex = 250, EndIndex = 350 },
            new() { GameTextId = hardTextDe.Id, LogicalFallacyId = middleGround.Id, StartIndex = 380, EndIndex = 460 },
            new() { GameTextId = hardTextDe.Id, LogicalFallacyId = loadedQuestion.Id, StartIndex = 530, EndIndex = 600 },
            new() { GameTextId = hardTextDe.Id, LogicalFallacyId = hastyGeneralization.Id, StartIndex = 600, EndIndex = 650 },
            new() { GameTextId = hardTextDe.Id, LogicalFallacyId = personalIncredulity.Id, StartIndex = 650, EndIndex = 780 }
        };

        context.TextFallacies.AddRange(easyTextEnFallacies);
        context.TextFallacies.AddRange(mediumTextEnFallacies);
        context.TextFallacies.AddRange(hardTextEnFallacies);
        context.TextFallacies.AddRange(easyTextDeFallacies);
        context.TextFallacies.AddRange(mediumTextDeFallacies);
        context.TextFallacies.AddRange(hardTextDeFallacies);
        await context.SaveChangesAsync();
    }

    private static async Task SeedLogicalFallacyTranslationsAsync(GameDbContext context)
    {
        var german = await context.Languages.FirstAsync(l => l.Code == "de");
        var fallacies = await context.LogicalFallacies.ToListAsync();

        var translations = new List<LogicalFallacyTranslation>();

        foreach (var fallacy in fallacies)
        {
            string germanName = fallacy.Name switch
            {
                "Ad Hominem" => "Ad Hominem",
                "Strawman" => "Strohmann-Argument",
                "Appeal to Authority" => "Autoritätsargument",
                "False Dilemma" => "Falsches Dilemma",
                "Slippery Slope" => "Dammbruch-Argument",
                "Circular Reasoning" => "Zirkelschluss",
                "Hasty Generalization" => "Voreilige Verallgemeinerung",
                "Red Herring" => "Roter Hering",
                "Appeal to Emotion" => "Appell an die Emotion",
                "Bandwagon" => "Mitläufer-Argument",
                "Tu Quoque" => "Tu quoque",
                "Appeal to Nature" => "Naturalistischer Fehlschluss",
                "The Fallacy Fallacy" => "Fehlschluss-Fehlschluss",
                "Appeal to Ignorance" => "Argumentum ad ignorantiam",
                "Composition/Division" => "Komposition/Division",
                "No True Scotsman" => "Kein wahrer Schotte",
                "Loaded Question" => "Suggestivfrage",
                "Begging the Question" => "Petitio principii",
                "Appeal to Consequences" => "Argumentum ad consequentiam",
                "Middle Ground" => "Goldene Mitte",
                "Burden of Proof" => "Beweislast",
                "Anecdotal Evidence" => "Anekdotische Evidenz",
                "Texas Sharpshooter" => "Texas-Scharfschütze",
                "Personal Incredulity" => "Persönliche Ungläubigkeit",
                _ => fallacy.Name,
            };

            string germanDescription = fallacy.Description switch
            {
                "Attacking the person making the argument rather than the argument itself." => "Angriff auf die Person, die das Argument vorbringt, anstatt auf das Argument selbst.",
                "Misrepresenting someone's argument to make it easier to attack." => "Falsche Darstellung eines Arguments, um es leichter angreifen zu können.",
                "Using the opinion of an authority figure as evidence for an argument." => "Verwendung der Meinung einer Autoritätsperson als Beweis für ein Argument.",
                "Presenting two alternative states as the only possibilities." => "Darstellung von zwei alternativen Zuständen als einzige Möglichkeiten.",
                "Asserting that one event will lead to a chain of negative events." => "Behauptung, dass ein Ereignis zu einer Kette negativer Ereignisse führen wird.",
                "The conclusion of an argument is used as a premise of that same argument." => "Die Schlussfolgerung eines Arguments wird als Prämisse desselben Arguments verwendet.",
                "Drawing a conclusion based on a small sample size." => "Ziehen einer Schlussfolgerung basierend auf einer kleinen Stichprobe.",
                "Introducing irrelevant information to divert attention from the main argument." => "Einführung irrelevanter Informationen, um die Aufmerksamkeit vom Hauptargument abzulenken.",
                "Manipulating emotions rather than using valid reasoning." => "Manipulation von Emotionen anstatt gültiger Argumentation.",
                "Appealing to popularity or the fact that many people do something." => "Berufung auf Popularität oder die Tatsache, dass viele Menschen etwas tun.",
                "Avoiding criticism by turning it back on the accuser." => "Vermeidung von Kritik durch Rückweisung an den Ankläger.",
                "Arguing that something is good because it's natural." => "Argumentation, dass etwas gut ist, weil es natürlich ist.",
                "Presuming that because a claim has been poorly argued it is therefore wrong." => "Annahme, dass eine Behauptung falsch ist, weil sie schlecht argumentiert wurde.",
                "Claiming something is true because it hasn't been proven false." => "Behauptung, dass etwas wahr ist, weil es nicht als falsch bewiesen wurde.",
                "Assuming what's true for a part is true for the whole, or vice versa." => "Annahme, dass was für einen Teil wahr ist, auch für das Ganze wahr ist, oder umgekehrt.",
                "Making an appeal to purity as a way to dismiss relevant criticisms." => "Berufung auf Reinheit als Weg, relevante Kritik abzuweisen.",
                "Asking a question that has an assumption built into it." => "Stellen einer Frage, die eine Annahme beinhaltet.",
                "A form of circular reasoning where the conclusion is assumed in the premise." => "Eine Form des Zirkelschlusses, bei dem die Schlussfolgerung in der Prämisse angenommen wird.",
                "Arguing for or against a position based solely on the consequences." => "Argumentation für oder gegen eine Position basierend ausschließlich auf den Konsequenzen.",
                "Assuming that the compromise between two positions is always correct." => "Annahme, dass der Kompromiss zwischen zwei Positionen immer richtig ist.",
                "Claiming that the burden of proof lies with someone else." => "Behauptung, dass die Beweislast bei jemand anderem liegt.",
                "Using personal experience or isolated examples instead of compelling evidence." => "Verwendung persönlicher Erfahrungen oder isolierter Beispiele anstatt überzeugender Beweise.",
                "Cherry-picking data clusters to suit an argument while ignoring significant data." => "Rosinenpicken von Datenclustern zur Unterstützung eines Arguments unter Ignorierung signifikanter Daten.",
                "Saying that because one finds something difficult to understand it's therefore false." => "Behauptung, dass etwas falsch ist, weil man es schwer zu verstehen findet.",
                _ => fallacy.Description,
            };

            translations.Add(new LogicalFallacyTranslation
            {
                LogicalFallacyId = fallacy.Id,
                LanguageId = german.Id,
                Name = germanName,
                Description = germanDescription
            });
        }

        context.LogicalFallacyTranslations.AddRange(translations);
        await context.SaveChangesAsync();
    }
}
