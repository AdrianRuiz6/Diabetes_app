using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_coins : MonoBehaviour
{
    private TMP_Text _coinsAmountText;

    private void Awake()
    {
        GameEventsEconomy.OnCoinsUpdated += OnCoinsUpdated;
        _coinsAmountText = GetComponent<TMP_Text>();
    }

    void OnDestroy()
    {
        GameEventsEconomy.OnCoinsUpdated -= OnCoinsUpdated;
    }

    private void OnCoinsUpdated(int coins)
    {
        _coinsAmountText.text = coins.ToString();
    }
}
