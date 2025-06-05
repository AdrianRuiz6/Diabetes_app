using Master.Domain.Connection;
using System;
using TMPro;
using UnityEngine;

namespace Master.Presentation.Connection
{
    public class UI_TimeChecker : MonoBehaviour
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

        private void OnDestroy()
        {
            ConnectionManager.SetDisconnectionDate(DateTime.Now);
        }
    }
}