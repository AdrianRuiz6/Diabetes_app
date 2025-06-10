using System;

public interface IScoreManager
{

    public int currentScore { get; }
    public int highestScore { get; }

    public void AddScore(int addedScore, DateTime? time);

    public void SubstractScore(int substractedScore, DateTime? time);

    public void ResetScore();
}
