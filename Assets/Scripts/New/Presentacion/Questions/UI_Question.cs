using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Question : MonoBehaviour
{
    private int _numberQuestionCounter;

    [SerializeField] private TMP_Text _numberQuestion_TMP;

    [SerializeField] private TMP_Text _questionTitle_TMP;

    [SerializeField] private Button _answer1_Btn;
    [SerializeField] private TMP_Text _answer1_TMP;

    [SerializeField] private Button _answer2_Btn;
    [SerializeField] private TMP_Text _answer2_TMP;

    [SerializeField] private Button _answer3_Btn;
    [SerializeField] private TMP_Text _answer3_TMP;

    [SerializeField] private GameObject _advicePanel;
    [SerializeField] private Button _openAdvice_Btn;
    [SerializeField] private Button _closeAdvice_Btn;
    [SerializeField] private TMP_Text _advice_TMP;

    [SerializeField] private Button _nextQuestion_Btn;

    void OnDestroy()
    {
        _answer1_Btn.onClick.RemoveAllListeners();
        _answer2_Btn.onClick.RemoveAllListeners();
        _answer3_Btn.onClick.RemoveAllListeners();

        _openAdvice_Btn.onClick.RemoveAllListeners();
        _closeAdvice_Btn.onClick.RemoveAllListeners();
        _nextQuestion_Btn.onClick.RemoveAllListeners();
    }

    private void OnEnable()
    {
        _numberQuestionCounter = 1;
    }

    void Start()
    {
        _answer1_Btn.onClick.AddListener(Answer1);
        _answer2_Btn.onClick.AddListener(Answer2);
        _answer3_Btn.onClick.AddListener(Answer3);

        _nextQuestion_Btn.onClick.AddListener(PrepareNextQuestion);
        _openAdvice_Btn.onClick.AddListener(ToggleAdvicePanel);
        _closeAdvice_Btn.onClick.AddListener(ToggleAdvicePanel);
    }

    private void PrepareNextQuestion()
    {
        _nextQuestion_Btn.gameObject.SetActive(false);
        _openAdvice_Btn.gameObject.SetActive(false);

        Question newQuestion = QuestionManager.Instance.NextQuestion();

        if(newQuestion == null)
        {
            GameEventsQuestions.OnFinalizedQuestions?.Invoke();
        }else
        {
            _numberQuestion_TMP.SetText("Pregunta número " + _numberQuestionCounter.ToString());
            _questionTitle_TMP.SetText(newQuestion.question);
            _answer1_TMP.SetText(newQuestion.answer1);
            _answer2_TMP.SetText(newQuestion.answer2);
            _answer3_TMP.SetText(newQuestion.answer3);
            _advice_TMP.SetText(newQuestion.advice);

            _numberQuestionCounter++;
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
        if(QuestionManager.Instance.GetCorrectAnswer().Equals(_answer1_TMP.ToString()))
        {
            // TODO: recompensa.
            CorrectAnswer(_answer1_Btn);
        }else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer2_TMP.ToString()))
        {
            WrongAnswer(_answer1_Btn);
            CorrectAnswer(_answer2_Btn);
        }else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer3_TMP.ToString()))
        {
            WrongAnswer(_answer1_Btn);
            CorrectAnswer(_answer3_Btn);
        }

        _nextQuestion_Btn.gameObject.SetActive(true);
        _openAdvice_Btn.gameObject.SetActive(true);
    }

    private void Answer2()
    {
        if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer1_TMP.ToString()))
        {
            WrongAnswer(_answer2_Btn);
            CorrectAnswer(_answer1_Btn);
        }
        else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer2_TMP.ToString()))
        {
            // TODO: recompensa.
            CorrectAnswer(_answer2_Btn);
        }
        else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer3_TMP.ToString()))
        {
            WrongAnswer(_answer2_Btn);
            CorrectAnswer(_answer3_Btn);
        }

        _nextQuestion_Btn.gameObject.SetActive(true);
        _openAdvice_Btn.gameObject.SetActive(true);
    }

    private void Answer3()
    {
        if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer1_TMP.ToString()))
        {
            WrongAnswer(_answer3_Btn);
            CorrectAnswer(_answer1_Btn);
        }
        else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer2_TMP.ToString()))
        {
            WrongAnswer(_answer3_Btn);
            CorrectAnswer(_answer2_Btn);
        }
        else if (QuestionManager.Instance.GetCorrectAnswer().Equals(_answer3_TMP.ToString()))
        {
            // TODO: recompensa.
            CorrectAnswer(_answer3_Btn);
        }

        _nextQuestion_Btn.gameObject.SetActive(true);
        _openAdvice_Btn.gameObject.SetActive(true);
    }

    private void CorrectAnswer(Button btn)
    {
        ColorBlock cb = btn.colors;

        cb.normalColor = Color.green;

        btn.colors = cb;
    }

    private void WrongAnswer(Button btn)
    {
        ColorBlock cb = btn.colors;

        cb.normalColor = Color.red;

        btn.colors = cb;
    }
}
