using Master.Domain.Questions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.GameEvents
{
    public static class GameEvents_Questions
    {
        // FSM work.
        public static Action<StateQuestions> OnExecuteNextQuestionState;
        public static Action OnFinishQuestionSearch;

        // UI work.
        public static Action OnActivateQuestionPanelUI;
        public static Action OnPrepareFirstQuestionUI;

        public static Action OnActivateTimerPanelUI;
        public static Action<float> OnPrepareTimerUI;

        public static Action OnActivateLoadingPanelUI;
        public static Action OnDeactivateLoadingPanelUI;
    }
}