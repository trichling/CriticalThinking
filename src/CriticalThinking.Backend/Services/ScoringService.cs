using CriticalThinking.Backend.Models;

namespace CriticalThinking.Backend.Services;

public interface IScoringService
{
    int CalculateScore(int correctAnswers, int wrongAnswers, int missedAnswers, int timeTakenSeconds, Difficulty difficulty);
}

public class ScoringService : IScoringService
{
    public int CalculateScore(int correctAnswers, int wrongAnswers, int missedAnswers, int timeTakenSeconds, Difficulty difficulty)
    {
        // Base scoring algorithm:
        // - Correct answers: +100 points each
        // - Wrong answers: -25 points each
        // - Missed answers: -50 points each
        // - Time bonus: based on difficulty and time taken
        // - Difficulty multiplier: Easy 1x, Medium 1.5x, Hard 2x

        var baseScore = (correctAnswers * 100) - (wrongAnswers * 25) - (missedAnswers * 50);
        
        // Ensure minimum score is 0
        baseScore = Math.Max(0, baseScore);

        // Time bonus calculation (faster = better)
        var timeBonus = CalculateTimeBonus(timeTakenSeconds, difficulty);
        
        // Apply difficulty multiplier
        var difficultyMultiplier = difficulty switch
        {
            Difficulty.Easy => 1.0,
            Difficulty.Medium => 1.5,
            Difficulty.Hard => 2.0,
            _ => 1.0
        };

        var finalScore = (int)((baseScore + timeBonus) * difficultyMultiplier);
        return Math.Max(0, finalScore);
    }

    private int CalculateTimeBonus(int timeTakenSeconds, Difficulty difficulty)
    {
        // Optimal time expectations based on difficulty
        var optimalTime = difficulty switch
        {
            Difficulty.Easy => 120,    // 2 minutes for 3 fallacies
            Difficulty.Medium => 300,  // 5 minutes for 6 fallacies
            Difficulty.Hard => 600,    // 10 minutes for 9 fallacies
            _ => 300
        };

        // If faster than optimal time, give bonus
        // If slower, reduce bonus (but don't penalize too much)
        if (timeTakenSeconds <= optimalTime)
        {
            // Bonus for being faster than optimal
            var timeRatio = (double)timeTakenSeconds / optimalTime;
            return (int)(50 * (1 - timeRatio)); // Max 50 points bonus
        }
        else
        {
            // Reduce bonus for being slower, but don't go negative
            var timeRatio = (double)optimalTime / timeTakenSeconds;
            return Math.Max(0, (int)(25 * timeRatio)); // Reduced bonus, minimum 0
        }
    }
}