using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestionsManager : MonoBehaviour
{
    [SerializeField] private GameObject _timerPanel;
    [SerializeField] private GameObject _questionPanel;
    [SerializeField] private GameObject _problemLoadingPanel;

    private bool _IsExistingProblemsLoading;

    void Awake()
    {
        // Events
        GameEventsQuestions.OnStartQuestionUI += ActivateQuestionsPanel;
        GameEventsQuestions.OnStartTimerUI += ActivateTimerPanel;
        GameEventsQuestions.OnFinalizedCreationQuestions += TurnFalseProblemsLoading;
    }

    void OnDestroy()
    {
        // Events
        GameEventsQuestions.OnStartQuestionUI -= ActivateQuestionsPanel;
        GameEventsQuestions.OnStartTimerUI -= ActivateTimerPanel;
        GameEventsQuestions.OnFinalizedCreationQuestions -= TurnFalseProblemsLoading;
    }

    private void Start()
    {
        _IsExistingProblemsLoading = true;

        DeactivateAllPanels();
        ActivateTimerPanel();
        GameEventsQuestions.OnStartTimerUI?.Invoke();
    }

    private void TurnFalseProblemsLoading()
    {
        _IsExistingProblemsLoading = false;
        if (_problemLoadingPanel.activeSelf == true)
        {
            ActivateQuestionsPanel();
        }
    }

    private void ActivateQuestionsPanel()
    {
        DeactivateAllPanels();
        if(_IsExistingProblemsLoading == true)
        {
            ActivateProblemLoadingPanel();
        }
        else
        {
            _questionPanel.SetActive(true);
        }
    }

    private void ActivateTimerPanel()
    {
        _IsExistingProblemsLoading = true;

        DeactivateAllPanels();
        _timerPanel.SetActive(true);
    }

    private void ActivateProblemLoadingPanel()
    {
        DeactivateAllPanels();
        _problemLoadingPanel.SetActive(true);
    }

    private void DeactivateAllPanels()
    {
        _timerPanel.SetActive(false);
        _questionPanel.SetActive(false);
        _problemLoadingPanel.SetActive(false);
    }
}