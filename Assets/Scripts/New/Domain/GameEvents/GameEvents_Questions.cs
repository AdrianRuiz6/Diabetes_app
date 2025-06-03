using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.GameEvents
{
    public static class GameEvents_Questions
    {
        // FSM work.
        public static Action OnExecuteQuestionSearch;
        public static Action OnFinalizedCreationQuestions;

        // UI work.
        public static Action OnStartQuestionUI;
        public static Action OnStartTimerUI;
        public static Action<float> OnModifyTimer;
        public static Action OnConfirmChangeQuestions;
    }
}