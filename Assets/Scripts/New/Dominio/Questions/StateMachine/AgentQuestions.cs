using UnityEngine;
using Master.Domain.GameEvents;


namespace Master.Domain.Questions
{
    public class AgentQuestions : MonoBehaviour
    {
        private StateQuestions currentState;
        public bool executing { get; private set; }

        void Awake()
        {
            GameEvents_Questions.OnExecuteQuestionSearch += ExecuteQuestionSearch;
            GameEvents_Questions.OnFinalizedCreationQuestions += FinalizeQuestionSearch;
        }

        private void OnDestroy()
        {
            GameEvents_Questions.OnExecuteQuestionSearch -= ExecuteQuestionSearch;
            GameEvents_Questions.OnFinalizedCreationQuestions -= FinalizeQuestionSearch;
        }

        void Start()
        {
            executing = false;
        }

        void Update()
        {
            if (executing == true)
            {
                currentState.Execute(this);
            }
        }

        public void Change_state(StateQuestions newState)
        {
            currentState.OnExit();
            currentState = newState;
        }

        private void ExecuteQuestionSearch()
        {
            currentState = new State_CheckUserPerformance();
            executing = true;
        }

        private void FinalizeQuestionSearch()
        {
            executing = false;
        }
    }
}