using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CurrentScore : MonoBehaviour
{
    private TMP_Text _currentScore_TMP;

    private void Awake()
    {
        GameEvents_Score.OnResetScore += ModifyOnMidnight;
        GameEvents_Score.OnModifyCurrentScore += ModifyCurrentScoreTMP;
    }

    private void OnDestroy()
    {
        GameEvents_Score.OnResetScore -= ModifyOnMidnight;
        GameEvents_Score.OnModifyCurrentScore -= ModifyCurrentScoreTMP;
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
        if(int.Parse(_currentScore_TMP.text) + addedScore < 0)
        {
            _currentScore_TMP.text = 0.ToString();
        }
        else
        {
            _currentScore_TMP.text = (int.Parse(_currentScore_TMP.text) + addedScore).ToString();
        }
    }

    private void ModifyOnMidnight()
    {
        _currentScore_TMP.text = 0.ToString();
    }
}
