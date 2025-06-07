using System.Collections.Generic;
using System;

namespace Master.Domain.PetCare
{
    public interface IPetCareRepository
    {
        #region Log

        #region ActionsLog
        public void SaveInsulinLog(List<ActionLog> insulinLogList);

        public List<ActionLog> LoadInsulinLog();

        public void SaveFoodLog(List<ActionLog> foodLogList);

        public List<ActionLog> LoadFoodLog();

        public void SaveExerciseLog(List<ActionLog> exerciseLogList);

        public List<ActionLog> LoadExerciseLog();

        #endregion

        #region AttributeLog
        public void SaveGlycemiaLog(List<AttributeLog> glycemiaLogList);

        public List<AttributeLog> LoadGlycemiaLog();

        public void SaveHungerLog(List<AttributeLog> hungerLogList);

        public List<AttributeLog> LoadHungerLog();

        public void SaveActivityLog(List<AttributeLog> activityLogList);

        public List<AttributeLog> LoadActivityLog();
        #endregion
        #endregion

        #region Actions
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