using System;

public interface IScoreManager
{

    public int currentScore { get; }
    public int highestScore { get; }

    public void AddScore(int addedScore, DateTime? time, string activity);

    public void SubstractScore(int substractedScore, DateTime? time, string activity);

    public void ResetScore();
}
