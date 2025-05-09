using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_HigherScore : MonoBehaviour
{
    private TMP_Text _higherScore_TMP;

    private void Awake()
    {
        GameEvents_Score.OnModifyHighestScore += ModifyHigherScoreTMP;
    }

    private void OnDestroy()
    {
        GameEvents_Score.OnModifyHighestScore -= ModifyHigherScoreTMP;
    }

    void Start()
    {
        _higherScore_TMP = GetComponent<TMP_Text>();
        _higherScore_TMP.text = DataStorage.LoadHighestScore().ToString();
    }

    private void ModifyHigherScoreTMP(int higherScore)
    {
        _higherScore_TMP.text = higherScore.ToString();
    }
}
