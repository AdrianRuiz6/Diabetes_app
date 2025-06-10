using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.GameEvents;
using Master.Domain.Questions;

namespace Master.Presentation.Questions
{
    public class UI_QuestionsTimer : MonoBehaviour
    {
        private float _timer;

        [SerializeField] private GameObject _timerPanel;
        [SerializeField] private Image _timerFillBar;
        [SerializeField] private Gradient _timerColorGradient;
        [SerializeField] private TMP_Text _timerTextTMP;

        private bool _isTimerActive;

        private IQuestionManager _questionManager;

        void Awake()
        {
            // Events
            GameEvents_Questions.OnPrepareTimerUI += StartTimer;
        }

        void OnDestroy()
        {
            // Events
            GameEvents_Questions.OnPrepareTimerUI -= StartTimer;
        }

        private void Start()
        {
            _questionManager = ServiceLocator.Instance.GetService<IQuestionManager>();
        }

        void Update()
        {
            if (_isTimerActive)
            {
                if (_timer <= 0)
                {
                    FinalizeTimer();
                }
                else
                {
                    _timer = Mathf.Clamp(_timer - Time.deltaTime, 0, _questionManager.maxTimerSeconds);
                    UpdateTimerBar();
                }
            }
        }

        private void StartTimer(float newTimerValue)
        {
            _isTimerActive = true;
            _timer = Mathf.Clamp(newTimerValue, 0, _questionManager.maxTimerSeconds);
            UpdateTimerBar();
        }

        private void FinalizeTimer()
        {
            _isTimerActive = false;
            _questionManager.FinishTimerQuestions();
        }

        private void TimerToClock()
        {
            int timerAux = (int)_timer;

            int hours = timerAux / 3600;
            timerAux -= hours * 3600;
            int minutes = timerAux / 60;
            timerAux -= minutes * 60;
            int seconds = timerAux;

            string hoursText = "";
            string minutesText = "";
            string secondsText = "";

            if (hours >= 10)
            {
                hoursText = hours.ToString();
            }
            else if (hours > 0 && hours < 10)
            {
                hoursText = "0" + hours.ToString();
            }
            else if (hours == 0)
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

            _timerTextTMP.text = hoursText + ":" + minutesText + ":" + secondsText;
        }

        private void UpdateTimerBar()
        {
            _timerFillBar.fillAmount = _timer / _questionManager.maxTimerSeconds;
            _timerFillBar.color = _timerColorGradient.Evaluate(_timerFillBar.fillAmount);

            TimerToClock();
        }
    }
}