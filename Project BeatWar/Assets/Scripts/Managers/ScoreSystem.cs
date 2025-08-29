using UnityEngine;

public static class ScoreSystem
{
    private const int MAX_SCORE = 1000000000;
    private const int PERFECT_SCORE = 1000;
    private const int GOOD_SCORE = 500;
    private const int MISS_SCORE = 0;

    public enum Judge
    {
        Perfect,
        Good,
        Miss
    }
    
    //=== runtime variables ===//
    public static int CurrentScore { get; private set; } = 0;
    public static int HighScore { get; private set; } = 0;
    
    private static Track currentTrack;
    
    public static void Reset(Track track)
    {
        CurrentScore = 0;
        currentTrack = track;
        if (track != null)
        {
            HighScore = track.HighScore;
        }
        else
        {
            HighScore = 0;
        }
    }

    public static void AddScore(Judge judge)
    {
        switch (judge)
        {
            case Judge.Perfect:
                CurrentScore += PERFECT_SCORE;
                break;
            case Judge.Good:
                CurrentScore += GOOD_SCORE;
                break;
            case Judge.Miss:
                CurrentScore += MISS_SCORE;
                break;
        }
        CurrentScore = Mathf.Clamp(CurrentScore, 0, MAX_SCORE);
        
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            if (currentTrack != null)
            {
                currentTrack.HighScore = HighScore;
                // Optionally save the high score to persistent storage here
                
                PlayerPrefs.SetInt("HighScore_" + currentTrack.name, HighScore);
            }
        }
    }
    
}