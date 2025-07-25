﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Master.Domain.GameEvents;
using Master.Domain.Score;
using Master.Infrastructure;

namespace Master.Presentation.Score
{
    public class UI_ScoreLog : MonoBehaviour
    {
        [SerializeField] private GameObject _logElementGameObject;

        private List<GameObject> _elementsList = new List<GameObject>();

        private IScoreLogManager _scoreLogManager;

        private bool _isSimulationFinished = false;

        void Awake()
        {
            GameEvents_Score.OnResetScoreLog += ClearElements;
            GameEvents_Score.OnAddScoreLog += AddElement;
            GameEvents_PetCare.OnFinishedSimulation += StartUI;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        void OnDestroy()
        {
            GameEvents_Score.OnResetScoreLog -= ClearElements;
            GameEvents_Score.OnAddScoreLog -= AddElement;
            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Score.OnResetScoreLog -= ClearElements;
                GameEvents_Score.OnAddScoreLog -= AddElement;
            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Score.OnResetScoreLog -= ClearElements;
            GameEvents_Score.OnAddScoreLog -= AddElement;
            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
        }
#endif

        void Start()
        {
            _scoreLogManager = ServiceLocator.Instance.GetService<IScoreLogManager>();
        }

        private void InitializeElements()
        {
            for (int index = 0; index < _scoreLogManager.scoreLogList.Count; index++)
            {
                ScoreLog scoreLog = _scoreLogManager.scoreLogList[index];
                AddElement(scoreLog, 0);
            }
        }

        private void ClearElements()
        {
            if (!_isSimulationFinished)
                return;

            foreach (GameObject element in _elementsList)
            {
                Destroy(element);
            }
        }

        private void AddElement(ScoreLog newScoreLog, int siblingIndex)
        {
            if (!_isSimulationFinished)
                return;

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

        private void StartUI()
        {
            _isSimulationFinished = true;

            if (_scoreLogManager.scoreLogList.Count > 0)
            {
                InitializeElements();
            }
        }
    }
}