using Master.Domain.GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Master.Presentation.Shop
{
    public class UI_Coins : MonoBehaviour
    {
        private TMP_Text _coinsAmountText;

        private void Awake()
        {
            GameEvents_Shop.OnTotalCoinsUpdated += OnCoinsUpdated;
            _coinsAmountText = GetComponent<TMP_Text>();
        }

        void OnDestroy()
        {
            GameEvents_Shop.OnTotalCoinsUpdated -= OnCoinsUpdated;
        }

        private void OnCoinsUpdated(int coins)
        {
            _coinsAmountText.text = coins.ToString();
        }
    }
}