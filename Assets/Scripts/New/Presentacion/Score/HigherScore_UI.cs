using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HigherScore_UI : MonoBehaviour
{
    private TMP_Text _higherScore_TMP;

    private void Awake()
    {
        GameEventsScore.OnModifyHighestScore += ModifyHigherScoreTMP;
    }

    private void OnDestroy()
    {
        GameEventsScore.OnModifyHighestScore -= ModifyHigherScoreTMP;
    }

    void Start()
    {
        _higherScore_TMP = GetComponent<TMP_Text>();
    }

    private void ModifyHigherScoreTMP(int higherScore)
    {
        _higherScore_TMP.text = higherScore.ToString();
    }
}
