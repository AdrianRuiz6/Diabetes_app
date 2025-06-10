using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Master.Domain.PetCare
{
    public interface IPetCareManager
    {
        // Simulation
        public float updateIntervalBTree { get; }
        public DateTime lastIterationBTreeStartTime { get; }

        // Last time actions used
        public DateTime? lastTimeInsulinUsed { get; }
        public DateTime? lastTimeExerciseUsed { get; }
        public DateTime? lastTimeFoodUsed { get; }

        // Actions in CD?
        public bool isInsulinActionInCD { get; set; }
        public bool isExerciseActionInCD { get; set; }
        public bool isFoodActionInCD { get; set; }

        // Attributes Ranges
        public List<AttributeState> glycemiaRangeStates { get; }
        public List<AttributeState> activityRangeStates { get; }
        public List<AttributeState> hungerRangeStates { get; }

        // Minimum and maximum attributes values
        public int minGlycemiaValue { get; }
        public int minActivityValue { get; }
        public int minHungerValue { get; }
        public int maxGlycemiaValue { get; }
        public int maxActivityValue { get; }
        public int maxHungerValue { get; }
        public int initialGlycemiaValue { get; }
        public int initialActivityValue { get; }
        public int initialHungerValue { get; }

        // Attributes values
        public int glycemiaValue { get; }
        public int activityValue { get; }
        public int hungerValue { get; }

        // Actions effects active?
        public bool isInsulinEffectActive { get; set; }
        public bool isExerciseEffectActive { get; set; }
        public bool isFoodEffectActive { get; set; }

        // Actions settings
        public float timeCDActions { get; }
        public float currentTimeCDInsulin { get; }
        public float currentTimeCDFood { get; }
        public float currentTimeCDExercise { get; }

        public float timeEffectActions { get; }

        public void ExecuteAttributesBTree();

        public void RestartGlycemia(DateTime? currentDateTime);

        public void RestartActivity(DateTime? currentDateTime);
        public void RestartHunger(DateTime? currentDateTime);

        public void ModifyGlycemia(int value, DateTime? currentDateTime = null, bool isCalledByAction = false);

        public void ModifyActivity(int value, DateTime? currentDateTime = null, bool isCalledByAction = false);

        public void ModifyHunger(int value, DateTime? currentDateTime = null, bool isCalledByAction = false);

        public bool IsGlycemiaInRange(AttributeRangeValue attributeStateRequested);

        public bool IsActivityInRange(AttributeRangeValue attributeStateRequested);

        public bool IsHungerInRange(AttributeRangeValue attributeStateRequested);

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

        public void SetLastIterationStartTime(DateTime newLastIterationTime);
    }
}