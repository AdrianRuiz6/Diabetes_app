using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Master.Persistence;
using Master.Persistence.Score;
using Master.Domain.GameEvents;
using Master.Persistence.Connection;

namespace Master.Presentation.Score
{
    public class UI_ScoreLog : MonoBehaviour
    {
        [SerializeField] private GameObject _logElement;
        private List<ScoreLogData> _infoList;

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

            GameEvents_Score.OnResetScore += ClearElements;
            GameEvents_Score.OnModifyCurrentScore += AddElement;
        }

        void OnDestroy()
        {
            DataStorage_Score.SaveScoreInfo(_infoList);

            GameEvents_Score.OnResetScore -= ClearElements;
            GameEvents_Score.OnModifyCurrentScore -= AddElement;
        }

        //void OnApplicationPause(bool pauseStatus)
        //{
        //    if (pauseStatus)
        //    {
        //        DataStorage.SaveScoreInfo(_infoList);

        //        GameEventsScore.OnResetScore -= ClearElements;
        //        GameEventsScore.OnModifyCurrentScore -= AddElement;
        //    }
        //}

        void Start()
        {
            _infoList = new List<ScoreLogData>();

            if (DataStorage_Connection.LoadDisconnectionDate().Date == DateTime.Now.Date)
            {
                InitializeElements();
            }
            else
            {
                ClearElements();
            }
        }

        private void ClearElements()
        {
            foreach (ScoreLogData scoreData in _infoList)
            {
                Destroy(scoreData.element);
            }

            _infoList.Clear();
        }

        public void AddElement(int addedScore, DateTime? time, string activity)
        {
            if (time.Value.Date == DateTime.Now.Date)
            {
                string sign = (addedScore >= 0) ? "+" : "";
                string info = $"{sign}{addedScore} por {activity}.";

                GameObject newElement = CreateLogElement(time.Value, info);
                ScoreLogData newLogData = new ScoreLogData(time.Value, info, newElement);

                // Se inserta el nuevo elemento en la posición correcta dentro de _infoList.
                int index = _infoList.FindLastIndex(log => log.GetTime() <= time.Value) + 1;
                if (index == -1)
                {
                    _infoList.Add(newLogData);
                }
                else
                {
                    _infoList.Insert(index, newLogData);
                }

                // Se inserta el nuevo elemento en la posición correcta en la jerarquía.
                int siblingIndex = _infoList.Count - index - 1;
                newElement.transform.SetSiblingIndex(siblingIndex);
            }
        }

        private void InitializeElements()
        {
            foreach (ScoreLogData scoreLog in DataStorage_Score.LoadScoreInfo())
            {
                GameObject newElement = CreateLogElement(scoreLog.GetTime(), scoreLog.GetInfo());
                ScoreLogData newLogData = new ScoreLogData(scoreLog.GetTime(), scoreLog.GetInfo(), newElement);
                _infoList.Add(newLogData);
            }
        }

        private GameObject CreateLogElement(DateTime time, string info)
        {
            GameObject newElement = Instantiate(_logElement);
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