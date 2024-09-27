using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.Events
{
    public static class GameEventsPetCare
    {
        public static Action <DateTime>OnExecutingAttributes;
        public static Action OnExecutedAttribute;
        public static Action<int, DateTime?> OnModifyGlycemia;
        public static Action<int, DateTime?> OnModifyActivity;
        public static Action<int, DateTime?> OnModifyHunger;
        public static Action<string, float> OnStartTimerCD;
    }
}