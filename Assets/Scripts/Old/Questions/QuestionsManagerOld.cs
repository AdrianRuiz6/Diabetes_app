using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsManagerOld : MonoBehaviour
{
    [Header("Explanation panel")]
    public Button openExplanationButton;
    public Button closeExplanationButton;
    public GameObject explanationPanel;

    [Header("Questions")]
    public GameObject objectIA;
    private QuestionGenerator questionGenerator;

    [Header("Next question")]
    public Button nextQuestionButton;
    public TMP_Text numberQuestionTMP;
    public TMP_Text questionTMP;
    public TMP_Text answer1TMP;
    public TMP_Text answer2TMP;
    public TMP_Text answer3TMP;
    public TMP_Text explanationAnswerTMP;

    [Header("Timer until next question")]
    public GameObject timerPanel;
    private float timer;
    public float timeUntilQuestions;
    public Image timerFillBar;
    public Gradient timerColorGradient;
    public TMP_Text timerTextTMP;    

    private void OnValidate()
    {
        timer = Mathf.Clamp(timer, 0, timeUntilQuestions);
    }

    void Start()
    {
        timer = timeUntilQuestions;
        ActivePanel();

        openExplanationButton.onClick.AddListener(OpenExplanationPanel);
        closeExplanationButton.onClick.AddListener(CloseExplanationPanel);

        nextQuestionButton.onClick.AddListener(NextQuestion);

        questionGenerator = objectIA.GetComponent<QuestionGenerator>();
    }

    void Update()
    {
        ActivePanel();
    }

    private void OpenExplanationPanel()
    {
        explanationPanel.SetActive(true);
    }

    private void CloseExplanationPanel()
    {
        explanationPanel.SetActive(false);
    }

    private void NextQuestion()
    {

    }

    private void ActivePanel()
    {
        if(timer > 0)
        {
            // User sees timer panel.
            if(timerPanel.activeSelf == false)
            {
                timerPanel.SetActive(true);
            }
            timer -= Time.deltaTime;
            UpdateTimerBar();
        }
        else
        {
            // User sees question panel.
            if(timerPanel.activeSelf == true)
            {
                timerPanel.SetActive(false);
            }
        }
    }

    private void TimerToClock()
    {
        int timerAux = (int)timer;

        int hours = timerAux/3600;
        timerAux -= hours*3600;
        int minutes = timerAux/60;
        timerAux -= minutes*60;
        int seconds = timerAux;

        string hoursText = "";
        string minutesText = "";
        string secondsText = "";

        if(hours >= 10)
        {
            hoursText = hours.ToString();
        }else if (hours > 0 && hours < 10)
        {
            hoursText = "0" + hours.ToString();
        }else if (hours == 0)
        {
            hoursText = "00";
        }

        if (minutes >= 10)
        {
            minutesText = minutes.ToString();
        }
        else if (minutes > 0 && minutes < 10)
        {
            minutesText = "0" + minutes.ToString();
        }
        else if (minutes == 0)
        {
            minutesText = "00";
        }

        if (seconds >= 10)
        {
            secondsText = seconds.ToString();
        }
        else if (seconds > 0 && seconds < 10)
        {
            secondsText = "0" + seconds.ToString();
        }
        else if (seconds == 0)
        {
            secondsText = "00";
        }

        timerTextTMP.text = hoursText + ":" + minutesText + ":" + secondsText;
    }

    public void UpdateTimerBar()
    {
        timer = Mathf.Clamp(timer, 0, timeUntilQuestions);
        timerFillBar.fillAmount = timer / timeUntilQuestions;
        timerFillBar.color = timerColorGradient.Evaluate(timerFillBar.fillAmount);

        TimerToClock();
    }
}
