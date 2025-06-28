using Master.Domain.Questions;
using System;
namespace Master.Domain.GameEvents
{
    public static class GameEvents_Questions
    {
        // FSM work.
        public static Action<StateQuestions> OnExecuteNextQuestionState;
        public static Action OnFinishQuestionSearch;

        // UI work.
        public static Action OnActivateQuestionPanelUI;

        public static Action OnActivateTimerPanelUI;
        public static Action<float> OnPrepareTimerUI;

        public static Action OnActivateLoadingPanelUI;
        public static Action OnDeactivateLoadingPanelUI;
    }
}