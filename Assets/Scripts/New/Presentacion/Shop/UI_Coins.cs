using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Coins : MonoBehaviour
{
    private TMP_Text _coinsAmountText;

    private void Awake()
    {
        GameEvents_Economy.OnTotalCoinsUpdated += OnCoinsUpdated;
        _coinsAmountText = GetComponent<TMP_Text>();
    }

    void OnDestroy()
    {
        GameEvents_Economy.OnTotalCoinsUpdated -= OnCoinsUpdated;
    }

    private void OnCoinsUpdated(int coins)
    {
        _coinsAmountText.text = coins.ToString();
    }
}
