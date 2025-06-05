using System.Collections.Generic;
using System;

namespace Master.Domain.PetCare
{
    public interface IPetCareRepository
    {
        #region Log

        #region ButtonLog
        public void SaveInsulinLog(DateTime? dateTime, string information);

        public Dictionary<DateTime, string> LoadInsulinLog(DateTime? requestedDate);

        public void SaveFoodLog(DateTime? dateTime, string information);

        public Dictionary<DateTime, string> LoadFoodLog(DateTime? requestedDate);

        public void SaveExerciseLog(DateTime? dateTime, string information);

        public Dictionary<DateTime, string> LoadExerciseLog(DateTime? requestedDate);

        public void ResetInsulinLog();

        public void ResetFoodLog();

        public void ResetExerciseLog();
        #endregion

        #region AttributeLog
        public void SaveGlycemiaLog(DateTime? dateTime, int number);

        public void ResetGlycemiaLog();

        public Dictionary<DateTime, int> LoadGlycemiaLog(DateTime? requestedDate);

        public void SaveHungerLog(DateTime? dateTime, int number);

        public void ResetHungerLog();

        public Dictionary<DateTime, int> LoadHungerLog(DateTime? requestedDate);

        public void SaveActivityLog(DateTime? dateTime, int number);

        public void ResetActivityLog();

        public Dictionary<DateTime, int> LoadActivityLog(DateTime? requestedDate);
        #endregion
        #endregion

        #region Buttons
        public void SaveLastTimeInsulinUsed(DateTime? lastTimeInsulinUsed);

        public DateTime? LoadLastTimeInsulinUsed();

        public void SaveLastTimeExerciseUsed(DateTime? lastTimeExerciseUsed);

        public DateTime? LoadLastTimeExerciseUsed();

        public void SaveLastTimeFoodUsed(DateTime? lastTimeFoodUsed);

        public DateTime? LoadLastTimeFoodUsed();
        #endregion

        #region Attributes
        public void SaveLastIterationStartTime(DateTime startTimeInterval);

        public DateTime LoadLastIterationStartTime();

        public void SaveGlycemia(int glycemiaValue);

        public int LoadGlycemia();

        public void SaveActivity(int activityValue);

        public int LoadActivity();

        public void SaveHunger(int hungerValue);

        public int LoadHunger();
        #endregion
    }
}