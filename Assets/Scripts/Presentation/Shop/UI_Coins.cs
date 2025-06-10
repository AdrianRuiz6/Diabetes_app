using Master.Domain.GameEvents;
using Master.Domain.Shop;
using TMPro;
using UnityEngine;

namespace Master.Presentation.Shop
{
    public class UI_Coins : MonoBehaviour
    {
        private TMP_Text _coinsAmountText;
        private IEconomyManager _economyManager;

        private void Awake()
        {
            GameEvents_Shop.OnTotalCoinsUpdated += OnCoinsUpdated;
            _coinsAmountText = GetComponent<TMP_Text>();
        }

        void OnDestroy()
        {
            GameEvents_Shop.OnTotalCoinsUpdated -= OnCoinsUpdated;
        }

        private void Start()
        {
            _economyManager = ServiceLocator.Instance.GetService<IEconomyManager>();

            OnCoinsUpdated(_economyManager.totalCoins);
        }

        private void OnCoinsUpdated(int coins)
        {
            _coinsAmountText.text = coins.ToString();
        }
    }
}