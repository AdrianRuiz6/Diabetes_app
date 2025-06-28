using System;

public class AttributeUpdateIntervalInfo
{
    public DateTime dateTime { get; private set; }
    public int glycemiaValue { get; private set; }
    public int energyValue { get; private set; }
    public int hungerValue { get; private set; }
    public bool isInsulinEffectActive { get; private set; }
    public bool isExerciseEffectActive { get; private set; }
    public bool isFoodEffectActive { get; private set; }

    public AttributeUpdateIntervalInfo(DateTime dateTime, int glycemiaValue, int energyValue, int hungerValue, bool isInsulinEffectActive, bool isExerciseEffectActive, bool isFoodEffectActive)
    {
        this.dateTime = dateTime;
        this.glycemiaValue = glycemiaValue;
        this.energyValue = energyValue;
        this.hungerValue = hungerValue;
        this.isInsulinEffectActive = isInsulinEffectActive;
        this.isExerciseEffectActive = isExerciseEffectActive;
        this.isFoodEffectActive = isFoodEffectActive;
    }
}
