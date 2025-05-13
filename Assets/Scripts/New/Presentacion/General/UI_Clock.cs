using System;
using TMPro;
using UnityEngine;

namespace Master.Presentation.General
{
    public class UI_Clock : MonoBehaviour
    {
        private TextMeshProUGUI clockText;

        private void Start()
        {
            clockText = GetComponent<TextMeshProUGUI>();

            DateTime currentTIme = DateTime.Now;
            float timeUntilNextMinute = 60f - currentTIme.Second;

            DisplayTime(currentTIme);
            Invoke(nameof(StartMinuteUpdates), timeUntilNextMinute);
        }

        private void StartMinuteUpdates()
        {
            InvokeRepeating(nameof(UpdateClock), 0f, 60f);
        }

        private void UpdateClock()
        {
            DateTime currentTime = DateTime.Now;
            DisplayTime(currentTime);
        }

        private void DisplayTime(DateTime time)
        {
            clockText.text = time.ToString("HH:mm");
        }
    }
}