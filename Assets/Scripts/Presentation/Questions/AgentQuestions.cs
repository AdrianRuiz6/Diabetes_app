using UnityEngine;
using Master.Domain.GameEvents;
using Master.Domain.Questions;
using Master.Infrastructure;

namespace Master.Presentation.Questions
{
    public class AgentQuestions : MonoBehaviour
    {
        private StateQuestions currentState;
        private IQuestionManager _questionManager;
        private IUserPerformanceManager _userPerformanceManager;

        void Awake()
        {
            GameEvents_Questions.OnExecuteNextQuestionState += ChangeState;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        private void OnDestroy()
        {
            GameEvents_Questions.OnExecuteNextQuestionState -= ChangeState;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Questions.OnExecuteNextQuestionState -= ChangeState;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Questions.OnExecuteNextQuestionState -= ChangeState;
        }
#endif

        void Start()
        {
            _questionManager = ServiceLocator.Instance.GetService<IQuestionManager>();
            _userPerformanceManager = ServiceLocator.Instance.GetService<IUserPerformanceManager>();

            currentState = new State_WaitForRestart(_questionManager, _userPerformanceManager);
        }

        void Update()
        {
            currentState.Execute();
        }

        public void ChangeState(StateQuestions newState)
        {
            currentState.OnExit();
            currentState = newState;
        }
    }
}