using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Master.Domain.PetCare
{
    public interface IPetCareManager
    {
        // Simulation
        public float updateIntervalBTree { get; }
        public DateTime nextIterationStartTime { get; }

        // Last time actions used
        public DateTime insulinCooldownEndTime { get; }
        public DateTime foodCooldownEndTime { get; }
        public DateTime exerciseCooldownEndTime { get; }

        public DateTime insulinEffectsEndTime { get; }
        public DateTime foodEffectsEndTime { get; }
        public DateTime exerciseEffectsEndTime { get; }

        // Actions in CD?
        public bool isInsulinActionInCD { get; set; }
        public bool isExerciseActionInCD { get; set; }
        public bool isFoodActionInCD { get; set; }

        // Attributes Ranges
        public List<AttributeState> glycemiaRangeStates { get; }
        public List<AttributeState> energyRangeStates { get; }
        public List<AttributeState> hungerRangeStates { get; }

        // Minimum and maximum attributes values
        public int minGlycemiaValue { get; }
        public int minEnergyValue { get; }
        public int minHungerValue { get; }
        public int maxGlycemiaValue { get; }
        public int maxEnergyValue { get; }
        public int maxHungerValue { get; }
        public int initialGlycemiaValue { get; }
        public int initialEnergyValue { get; }
        public int initialHungerValue { get; }

        // Attributes values
        public int glycemiaValue { get; }
        public int energyValue { get; }
        public int hungerValue { get; }

        // Actions effects active?
        public bool isInsulinEffectActive { get; set; }
        public bool isExerciseEffectActive { get; set; }
        public bool isFoodEffectActive { get; set; }

        // Actions settings
        public float timeCDActions { get; }

        public float timeEffectActions { get; }

        public void ExecuteAttributesBTree();

        public void RestartGlycemia(DateTime? currentDateTime);

        public void RestartEnergy(DateTime? currentDateTime);
        public void RestartHunger(DateTime? currentDateTime);

        public void StartStashGlycemia();

        public void ApplyStashedGlycemia(DateTime? currentDateTime);

        public void ModifyGlycemia(int value, DateTime? currentDateTime = null, bool isCalledByAction = false);

        public void StartStashEnergy();

        public void ApplyStashedEnergy(DateTime? currentDateTime);

        public void ModifyEnergy(int value, DateTime? currentDateTime = null, bool isCalledByAction = false);

        public void StartStashHunger();

        public void ApplyStashedHunger(DateTime? currentDateTime);

        public void ModifyHunger(int value, DateTime? currentDateTime = null, bool isCalledByAction = false);

        public bool IsGlycemiaInRange(AttributeRangeValue attributeStateRequested, int currentGlycemiaValue);

        public bool IsEnergyInRange(AttributeRangeValue attributeStateRequested, int currentEnergyValue);

        public bool IsHungerInRange(AttributeRangeValue attributeStateRequested, int currentHungerValue);

        public void ActivateInsulinAction(int value);

        public void DeactivateInsulinActionCD();

        public void DeactivateInsulinEffect();

        public void ActivateExerciseAction(string intensity);

        public void DeactivateExerciseActionCD();

        public void DeactivateExerciseEffect();

        public void ActivateFoodAction(float ration, string food);

        public void DeactivateFoodActionCD();

        public void DeactivateFoodEffect();

        public Task<string> GetInformationFromFoodName(string foodName);

        public float ExtractRationsFromText(string text);

        public void SetNextIterationStartTime(DateTime newNextIterationStartTime);
    }
}