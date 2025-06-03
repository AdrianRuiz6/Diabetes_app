using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.GameEvents
{
    public static class GameEvents_PetCare
    {
        public static Action <DateTime>OnExecutingAttributes;
        public static Action OnExecutedAttribute;
        public static Action<int, DateTime?, bool> OnModifyGlycemia;
        public static Action<int, DateTime?, bool> OnModifyActivity;
        public static Action<int, DateTime?, bool> OnModifyHunger;
        public static Action<string, float> OnStartTimerCD;
        public static Action OnFinishTimerCD;
    }
}