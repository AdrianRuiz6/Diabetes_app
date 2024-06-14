using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentScore_UI : MonoBehaviour
{
    private TMP_Text _currentScore_TMP;

    private void Awake()
    {
        GameEventsScore.OnModifyCurrentScore += ModifyCurrentScoreTMP;
    }

    private void OnDestroy()
    {
        GameEventsScore.OnModifyCurrentScore -= ModifyCurrentScoreTMP;
    }

    void Start()
    {
        _currentScore_TMP = GetComponent<TMP_Text>();
    }

    private void ModifyCurrentScoreTMP(int currentScore)
    {
        _currentScore_TMP.text = currentScore.ToString();
    }
}
