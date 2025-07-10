using Master.Domain.Connection;
using Master.Infrastructure;
using System;
using TMPro;
using UnityEngine;

namespace Master.Presentation.Connection
{
    public class UI_TimeChecker : MonoBehaviour
    {
        private TextMeshProUGUI clockText;

        private IConnectionManager _connectionManager;

        private void Start()
        {
            _connectionManager = ServiceLocator.Instance.GetService<IConnectionManager>();

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

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        private void OnDestroy()
        {
            _connectionManager.SetDisconnectionDate(DateTime.Now);
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                _connectionManager.SetDisconnectionDate(DateTime.Now);
            }
        }

        void OnApplicationQuit()
        {
            _connectionManager.SetDisconnectionDate(DateTime.Now);
        }
#endif
    }
}