using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentScore_UI : MonoBehaviour
{
    private TMP_Text _currentScore_TMP;

    private void Awake()
    {
        GameEventsScore.OnMidnight += ModifyOnMidnight;
        GameEventsScore.OnModifyCurrentScore += ModifyCurrentScoreTMP;
    }

    private void OnDestroy()
    {
        GameEventsScore.OnMidnight -= ModifyOnMidnight;
        GameEventsScore.OnModifyCurrentScore -= ModifyCurrentScoreTMP;
    }

    void Start()
    {
        _currentScore_TMP = GetComponent<TMP_Text>();

        if (DataStorage.LoadDisconnectionDate().Date == DateTime.Now.Date)
        {
            _currentScore_TMP.text = DataStorage.LoadCurrentScore().ToString();
        }
        else
        {
            _currentScore_TMP.text = 0.ToString();
        }
        
    }

    private void ModifyCurrentScoreTMP(int addedScore, DateTime? time, string info)
    {
        _currentScore_TMP.text = (int.Parse(_currentScore_TMP.text) + addedScore).ToString();
    }

    private void ModifyOnMidnight()
    {
        _currentScore_TMP.text = 0.ToString();
    }
}
