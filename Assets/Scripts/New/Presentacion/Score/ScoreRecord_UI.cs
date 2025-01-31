using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreRecord_UI : MonoBehaviour
{
    [SerializeField] private GameObject _recordElement;
    private List<ScoreRecordData> _infoList;

    private int _currenScore;

    public static ScoreRecord_UI Instance;

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

        GameEventsScore.OnResetScore += ClearElements;
        GameEventsScore.OnModifyCurrentScore += AddElement;
    }

    void OnDestroy()
    {
        DataStorage.SaveScoreInfo(_infoList);

        GameEventsScore.OnResetScore -= ClearElements;
        GameEventsScore.OnModifyCurrentScore -= AddElement;
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
        _infoList = new List<ScoreRecordData>();
        
        if(DataStorage.LoadDisconnectionDate().Date == DateTime.Now.Date)
        {
            InitializeElements();
            _currenScore = DataStorage.LoadCurrentScore();
        }
        else
        {
            ClearElements();
            _currenScore = 0;
        }
    }

    private void ClearElements()
    {
        foreach(ScoreRecordData scoreData in _infoList)
        {
            Destroy(scoreData.element);
        }

        _infoList.Clear();
    }

    public void AddElement(int addedScore, DateTime? time, string activity)
    {
        if (time.Value.Date == DateTime.Now.Date)
        {
            _currenScore += addedScore;

            string sign = (addedScore >= 0) ? "+" : "";
            string info = $"{sign}{addedScore} por {activity}.";

            GameObject newElement = CreateRecordElement(time.Value, info);
            ScoreRecordData newRecordData = new ScoreRecordData(time.Value, info, newElement);

            // Se inserta el nuevo elemento en la posición correcta dentro de _infoList.
            int index = _infoList.FindLastIndex(record => record.GetTime() <= time.Value) + 1;
            if (index == -1)
            {
                _infoList.Add(newRecordData);
            }
            else
            {
                _infoList.Insert(index, newRecordData);
            }

            // Se inserta el nuevo elemento en la posición correcta en la jerarquía.
            int siblingIndex = _infoList.Count - index - 1;
            newElement.transform.SetSiblingIndex(siblingIndex);
        }
    }

    private void InitializeElements()
    {
        foreach (ScoreRecordData scoreRecord in DataStorage.LoadScoreInfo())
        {
            GameObject newElement = CreateRecordElement(scoreRecord.GetTime(), scoreRecord.GetInfo());
            ScoreRecordData newRecordData = new ScoreRecordData(scoreRecord.GetTime(), scoreRecord.GetInfo(), newElement);
            _infoList.Add(newRecordData);
        }
    }

    private GameObject CreateRecordElement(DateTime time, string info)
    {
        GameObject newElement = Instantiate(_recordElement);
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
