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
        GameEventsEconomy.OnTotalCoinsUpdated += OnCoinsUpdated;
        _coinsAmountText = GetComponent<TMP_Text>();
    }

    void OnDestroy()
    {
        GameEventsEconomy.OnTotalCoinsUpdated -= OnCoinsUpdated;
    }

    private void OnCoinsUpdated(int coins)
    {
        _coinsAmountText.text = coins.ToString();
    }
}
