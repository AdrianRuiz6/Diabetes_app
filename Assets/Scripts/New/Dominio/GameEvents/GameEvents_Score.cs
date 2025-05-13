using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.GameEvents
{
    public static class GameEvents_Score
    {
        public static Action<int, DateTime?, string> OnModifyCurrentScore;
        public static Action<int> OnModifyHighestScore;
        public static Action OnResetScore;
    }
}