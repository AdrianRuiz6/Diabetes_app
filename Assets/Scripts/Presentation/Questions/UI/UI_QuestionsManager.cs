using UnityEngine;
using Master.Domain.GameEvents;
using System.Collections;
using Master.Domain.Questions;
using Master.Infrastructure;

namespace Master.Presentation.Questions
{
    public class UI_QuestionsManager : MonoBehaviour
    {
        [SerializeField] private GameObject _timerPanel;
        [SerializeField] private GameObject _questionPanel;
        [SerializeField] private GameObject _loadingPanel;

        private enum PanelType { Timer, Question }

        private IQuestionManager _questionManager;

        void Awake()
        {
            GameEvents_Questions.OnActivateQuestionPanelUI += ActivateQuestionPanel;
            GameEvents_Questions.OnActivateTimerPanelUI += ActivateTimerPanel;

            GameEvents_Questions.OnActivateLoadingPanelUI += StartLoadingQuestions;
            GameEvents_Questions.OnDeactivateLoadingPanelUI += FinishLoadingQuestions;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        void OnDestroy()
        {
            GameEvents_Questions.OnActivateQuestionPanelUI -= ActivateQuestionPanel;
            GameEvents_Questions.OnActivateTimerPanelUI -= ActivateTimerPanel;

            GameEvents_Questions.OnActivateLoadingPanelUI -= StartLoadingQuestions;
            GameEvents_Questions.OnDeactivateLoadingPanelUI -= FinishLoadingQuestions;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Questions.OnActivateQuestionPanelUI -= ActivateQuestionPanel;
                GameEvents_Questions.OnActivateTimerPanelUI -= ActivateTimerPanel;

                GameEvents_Questions.OnActivateLoadingPanelUI -= StartLoadingQuestions;
                GameEvents_Questions.OnDeactivateLoadingPanelUI -= FinishLoadingQuestions;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Questions.OnActivateQuestionPanelUI -= ActivateQuestionPanel;
            GameEvents_Questions.OnActivateTimerPanelUI -= ActivateTimerPanel;

            GameEvents_Questions.OnActivateLoadingPanelUI -= StartLoadingQuestions;
            GameEvents_Questions.OnDeactivateLoadingPanelUI -= FinishLoadingQuestions;
        }
#endif

        private void Start()
        {
            _questionManager = ServiceLocator.Instance.GetService<IQuestionManager>();

            StartCoroutine(TryInitializeQuestions());
        }

        private IEnumerator TryInitializeQuestions()
        {
            while (true)
            {
                bool result = _questionManager.InitializeQuestions();
                if (result)
                {
                    yield break;
                }

                yield return new WaitForSeconds(30f);
            }
        }

        private void StartLoadingQuestions()
        {
            _loadingPanel.SetActive(true);
        }

        private void FinishLoadingQuestions()
        {
            _loadingPanel.SetActive(false);
        }

        private void ActivateQuestionPanel()
        {
            SwitchPanel(PanelType.Question);
        }

        private void ActivateTimerPanel()
        {
            SwitchPanel(PanelType.Timer);
        }

        private void SwitchPanel(PanelType panel)
        {
            if (panel == PanelType.Timer)
            {
                _timerPanel.SetActive(true);
                _questionPanel.SetActive(false);
            }
            else if (panel == PanelType.Question)
            {
                _timerPanel.SetActive(false);
                _questionPanel.SetActive(true);
            }
        }
    }
}