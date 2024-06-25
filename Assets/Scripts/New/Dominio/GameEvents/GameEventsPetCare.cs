using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.Events
{
    public static class GameEventsPetCare
    {
        public static Action OnExecutingAttributes;
        public static Action OnExecutedAttribute;
        public static Action<int> OnModifyGlycemia;
        public static Action<int> OnModifyActivity;
        public static Action<int> OnModifyHunger;
        public static Action<string> OnActivateCoolDown;
        public static Action<string> OnDeactivateCoolDown;
    }
}