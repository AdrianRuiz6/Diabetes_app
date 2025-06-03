using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Presentation.PetCare
{
    public enum AttributeRangeValue
    {
        Good,
        Intermediate,
        Bad,
        Critical
    }
}

namespace Master.Presentation.PetCare
{
    [System.Serializable]
    public class AttributeState
    {
        public string Name;
        public int MinValue;
        public int MaxValue;
        public Color StateColor;
        public AttributeRangeValue RangeValue;
    }
}