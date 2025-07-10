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

        public void SaveEnergyLog(List<AttributeLog> energyLogList);

        public List<AttributeLog> LoadEnergyLog();
        #endregion
        #endregion

        #region Actions
        public void SaveInsulinCooldownEndTime(DateTime insulinCooldownEndTime);

        public DateTime LoadInsulinCooldownEndTime();

        public void SaveInsulinEffectsEndTime(DateTime insulinEffectsEndTime);

        public DateTime LoadInsulinEffectsEndTime();

        public void SaveExerciseCooldownEndTime(DateTime exerciseCooldownEndTime);

        public DateTime LoadExerciseCooldownEndTime();

        public void SaveExerciseEffectsEndTime(DateTime exerciseEffectsEndTime);

        public DateTime LoadExerciseEffectsEndTime();

        public void SaveFoodCooldownEndTime(DateTime foodCooldownEndTime);

        public DateTime LoadFoodCooldownEndTime();

        public void SaveFoodEffectsEndTime(DateTime foodEffectsEndTime);

        public DateTime LoadFoodEffectsEndTime();
        #endregion

        #region Attributes
        public void SaveNextIterationStartTime(DateTime startTimeInterval);

        public DateTime LoadNextIterationStartTime();

        public void SaveGlycemia(int glycemiaValue);

        public int LoadGlycemia();

        public void SaveEnergy(int EnergyValue);

        public int LoadEnergy();

        public void SaveHunger(int hungerValue);

        public int LoadHunger();
        #endregion
    }
}