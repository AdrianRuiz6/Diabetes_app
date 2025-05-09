using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestionsManager : MonoBehaviour
{
    [SerializeField] private GameObject _timerPanel;
    [SerializeField] private GameObject _questionPanel;
    [SerializeField] private GameObject _loadingPanel;

    private bool _isLoadingQuestions;
    private enum PanelType { Timer, Question }

    void Awake()
    {
        // Events
        GameEvents_Questions.OnStartQuestionUI += ActivateQuestionPanel;
        GameEvents_Questions.OnStartTimerUI += ActivateTimerPanel;
        GameEvents_Questions.OnFinalizedCreationQuestions += FinishLoadingQuestions;
    }

    void OnDestroy()
    {
        // Events
        GameEvents_Questions.OnStartQuestionUI -= ActivateQuestionPanel;
        GameEvents_Questions.OnStartTimerUI -= ActivateTimerPanel;
        GameEvents_Questions.OnFinalizedCreationQuestions -= FinishLoadingQuestions;
    }

    private void Start()
    {
        _isLoadingQuestions = true;

        SwitchPanel(PanelType.Timer);

        float previousTimeLeft = DataStorage.LoadTimeLeftQuestionTimer();
        DateTime lastTimeDisconnected = DataStorage.LoadDisconnectionDate();

        TimeSpan timeDisconnected = DateTime.Now - lastTimeDisconnected;
        float currentTimeLeft = previousTimeLeft - (float)timeDisconnected.TotalSeconds;
        GameEvents_Questions.OnModifyTimer?.Invoke(currentTimeLeft);
    }

    private void Update()
    {
        if (_isLoadingQuestions)
        {
            _loadingPanel.SetActive(true);
        }
        else
        {
            _loadingPanel.SetActive(false);
        }
    }

    private void StartLoadingQuestions()
    {
        _isLoadingQuestions = true;
    }

    private void FinishLoadingQuestions()
    {
        _isLoadingQuestions = false;
    }

    private void ActivateQuestionPanel()
    {
        SwitchPanel(PanelType.Question);
    }

    private void ActivateTimerPanel()
    {
        StartLoadingQuestions();
        SwitchPanel(PanelType.Timer);
    }

    private void SwitchPanel(PanelType panel)
    {
        if (panel == PanelType.Timer)
        {
            _timerPanel.SetActive(true);
            _questionPanel.SetActive(false);
            _loadingPanel.SetActive(false);
        }
        else if (panel == PanelType.Question)
        {
            _timerPanel.SetActive(false);
            _questionPanel.SetActive(true);
            _loadingPanel.SetActive(false);
        }
    }
}