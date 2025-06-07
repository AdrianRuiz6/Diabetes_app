using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Master.Persistence.Score;
using Master.Domain.GameEvents;
using Master.Persistence.Connection;

namespace Master.Presentation.Score
{
    public class UI_ScoreLog : MonoBehaviour
    {
        [SerializeField] private GameObject _logElementGameObject;

        private List<GameObject> _elementsList = new List<GameObject>();

        public static UI_ScoreLog Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            GameEvents_Score.OnResetScoreLog += ClearElements;
            GameEvents_Score.OnAddScoreLog += AddElement;
        }

        void OnDestroy()
        {
            GameEvents_Score.OnResetScoreLog -= ClearElements;
            GameEvents_Score.OnAddScoreLog -= AddElement;
        }

        void Start()
        {
            if (ScoreLogManager.scoreLogList.Count > 0)
            {
                InitializeElements();
            }
        }

        private void InitializeElements()
        {
            foreach (ScoreLog scoreLog in ScoreLogManager.scoreLogList)
            {
                int index = ScoreLogManager.scoreLogList.FindLastIndex(log => log.GetTime() <= time.Value) + 1;
                int siblingIndex = ScoreLogManager.scoreLogList.Count - index - 1;

                AddElement(scoreLog, siblingIndex);
            }
        }

        private void ClearElements()
        {
            foreach(GameObject element in _elementsList)
            {
                Destroy(element);
            }
        }

        private void AddElement(ScoreLog newScoreLog, int siblingIndex)
        {
            GameObject newElement = CreateLogElement(newScoreLog.GetTime(), newScoreLog.GetInfo());
            newElement.transform.SetSiblingIndex(siblingIndex);
            _elementsList.Add(newElement);
        }

        private GameObject CreateLogElement(DateTime time, string info)
        {
            GameObject newElement = Instantiate(_logElementGameObject);
            newElement.transform.SetParent(transform, false);
            newElement.transform.SetSiblingIndex(0);

            TextMeshProUGUI infoTMP = newElement.transform.Find("ScoreElement/Info_TMP").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeTMP = newElement.transform.Find("ScoreElement/Time_TMP").GetComponent<TextMeshProUGUI>();

            string textHour = (time.Hour > 9) ? time.Hour.ToString() : $"0{time.Hour}";
            string textMinute = (time.Minute > 9) ? time.Minute.ToString() : $"0{time.Minute}";
            timeTMP.text = $"{textHour}:{textMinute}";
            infoTMP.text = info;

            return newElement;
        }
    }
}