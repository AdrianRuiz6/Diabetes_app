using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeRangeValue
{
    Good,
    Intermediate,
    Bad
}

[System.Serializable]
public class AttributeState
{
    public string Name;
    public int MinValue;
    public int MaxValue;
    public Color StateColor;
    public AttributeRangeValue RangeValue;
}
