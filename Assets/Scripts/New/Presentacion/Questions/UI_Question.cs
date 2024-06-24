using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Master.Domain.Events;
using Master.Domain.Economy;

public class UI_Question : MonoBehaviour
{
    private Question currentQuestion;

    [SerializeField] private TMP_Text _numberQuestion_TMP;

    [SerializeField] private TMP_Text _questionTitle_TMP;

    [SerializeField] private GameObject _answer1_Object;
    private Button _answer1_Btn;
    private Image _answer1_Image;
    [SerializeField] private TMP_Text _answer1_TMP;

    [SerializeField] private GameObject _answer2_Object;
    private Button _answer2_Btn;
    private Image _answer2_Image;
    [SerializeField] private TMP_Text _answer2_TMP;

    [SerializeField] private GameObject _answer3_Object;
    private Button _answer3_Btn;
    private Image _answer3_Image;
    [SerializeField] private TMP_Text _answer3_TMP;

    [SerializeField] private GameObject _advicePanel;
    [SerializeField] private Button _openAdvice_Btn;
    [SerializeField] private Button _closeAdvice_Btn;
    [SerializeField] private TMP_Text _advice_TMP;

    [SerializeField] private Button _nextQuestion_Btn;

    void Awake()
    {
        GameEventsQuestions.OnFinalizedCreationQuestions += PrepareNextQuestion;
    }

    void OnDestroy()
    {
        // Buttons
        _answer1_Btn.onClick.RemoveAllListeners();
        _answer2_Btn.onClick.RemoveAllListeners();
        _answer3_Btn.onClick.RemoveAllListeners();

        _openAdvice_Btn.onClick.RemoveAllListeners();
        _closeAdvice_Btn.onClick.RemoveAllListeners();
        _nextQuestion_Btn.onClick.RemoveAllListeners();

        // Events
        GameEventsQuestions.OnFinalizedCreationQuestions -= PrepareNextQuestion;
    }

    void Start()
    {
        _answer1_Btn = _answer1_Object.GetComponent<Button>();
        _answer1_Image = _answer1_Object.GetComponent<Image>();
        _answer2_Btn = _answer2_Object.GetComponent<Button>();
        _answer2_Image = _answer2_Object.GetComponent<Image>();
        _answer3_Btn = _answer3_Object.GetComponent<Button>();
        _answer3_Image = _answer3_Object.GetComponent<Image>();

        _answer1_Btn.onClick.AddListener(Answer1);
        _answer2_Btn.onClick.AddListener(Answer2);
        _answer3_Btn.onClick.AddListener(Answer3);

        _nextQuestion_Btn.onClick.AddListener(PrepareNextQuestion);
        _openAdvice_Btn.onClick.AddListener(ToggleAdvicePanel);
        _closeAdvice_Btn.onClick.AddListener(ToggleAdvicePanel);
    }

    private void PrepareNextQuestion()
    {
        ActivateTargetableAllButtons();
        RestartColorAllButtons();

        _nextQuestion_Btn.gameObject.SetActive(false);
        _openAdvice_Btn.gameObject.SetActive(false);

        currentQuestion = QuestionManager.Instance.NextQuestion();

        if(currentQuestion == null)
        {
            GameEventsQuestions.OnStartTimerUI?.Invoke();
        }else
        {
            _numberQuestion_TMP.SetText((QuestionManager.Instance.currentQuestionIndex).ToString());
            _questionTitle_TMP.SetText(currentQuestion.question);
            _answer1_TMP.SetText(currentQuestion.answer1);
            _answer2_TMP.SetText(currentQuestion.answer2);
            _answer3_TMP.SetText(currentQuestion.answer3);
            _advice_TMP.SetText(currentQuestion.advice);

            Debug.Log("Pregunta número " + QuestionManager.Instance.currentQuestionIndex + ": " + currentQuestion.topic);
        }
    }

    private void ToggleAdvicePanel()
    {
        if (_advicePanel.activeSelf == true)
        {
            _advicePanel.SetActive(false);
        }
        else
        {
            _advicePanel.SetActive(true);
        }
    }

    private void Answer1()
    {
        currentQuestion.answerQuestion(_answer1_TMP.text);
        DeactivateTargetableAllButtons();

        if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer1_TMP.text))
        {
            // Recompensa.
            EconomyManager.Instance.AddCoins(50);
            ScoreManager.Instance.AddScore(50);
            CorrectAnswer(_answer1_Image);
        }else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer2_TMP.text))
        {
            ScoreManager.Instance.SubstractScore(25);
            WrongAnswer(_answer1_Image);
            CorrectAnswer(_answer2_Image);
        }else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer3_TMP.text))
        {
            ScoreManager.Instance.SubstractScore(25);
            WrongAnswer(_answer1_Image);
            CorrectAnswer(_answer3_Image);
        }

        _nextQuestion_Btn.gameObject.SetActive(true);
        _openAdvice_Btn.gameObject.SetActive(true);
    }

    private void Answer2()
    {
        currentQuestion.answerQuestion(_answer2_TMP.text);
        DeactivateTargetableAllButtons();

        if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer1_TMP.text))
        {
            ScoreManager.Instance.SubstractScore(25);
            WrongAnswer(_answer2_Image);
            CorrectAnswer(_answer1_Image);
        }
        else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer2_TMP.text))
        {
            // Recompensa.
            EconomyManager.Instance.AddCoins(50);
            ScoreManager.Instance.AddScore(50);
            CorrectAnswer(_answer2_Image);
        }
        else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer3_TMP.text))
        {
            ScoreManager.Instance.SubstractScore(25);
            WrongAnswer(_answer2_Image);
            CorrectAnswer(_answer3_Image);
        }

        _nextQuestion_Btn.gameObject.SetActive(true);
        _openAdvice_Btn.gameObject.SetActive(true);
    }

    private void Answer3()
    {
        currentQuestion.answerQuestion(_answer3_TMP.text);
        DeactivateTargetableAllButtons();

        if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer1_TMP.text))
        {
            ScoreManager.Instance.SubstractScore(25);
            WrongAnswer(_answer3_Image);
            CorrectAnswer(_answer1_Image);
        }
        else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer2_TMP.text))
        {
            ScoreManager.Instance.SubstractScore(25);
            WrongAnswer(_answer3_Image);
            CorrectAnswer(_answer2_Image);
        }
        else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer3_TMP.text))
        {
            // Recompensa.
            EconomyManager.Instance.AddCoins(50);
            ScoreManager.Instance.AddScore(50);
            CorrectAnswer(_answer3_Image);
        }

        _nextQuestion_Btn.gameObject.SetActive(true);
        _openAdvice_Btn.gameObject.SetActive(true);
    }

    private void CorrectAnswer(Image img)
    {
        img.color = Color.green;
    }

    private void WrongAnswer(Image img)
    {
        img.color = Color.red;
    }

    private void ActivateTargetableAllButtons()
    {
        _answer1_Btn.interactable = true;
        _answer2_Btn.interactable = true;
        _answer3_Btn.interactable = true;
    }

    private void DeactivateTargetableAllButtons()
    {
        _answer1_Btn.interactable = false;
        _answer2_Btn.interactable = false;
        _answer3_Btn.interactable = false;
    }

    private void RestartColorAllButtons()
    {
        _answer1_Image.color = Color.white;
        _answer2_Image.color = Color.white;
        _answer3_Image.color = Color.white;
    }
}
