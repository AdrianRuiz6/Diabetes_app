using Master.Domain.GameEvents;
using Master.Domain.Shop;
using TMPro;
using UnityEngine;
using Master.Infrastructure;

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

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        void OnDestroy()
        {
            GameEvents_Shop.OnTotalCoinsUpdated -= OnCoinsUpdated;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Shop.OnTotalCoinsUpdated -= OnCoinsUpdated;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Shop.OnTotalCoinsUpdated -= OnCoinsUpdated;
        }
#endif

        private void Start()
        {
            _economyManager = ServiceLocator.Instance.GetService<IEconomyManager>();

            OnCoinsUpdated();
        }

        private void OnCoinsUpdated()
        {
            _coinsAmountText.text = _economyManager.totalCoins.ToString();
        }
    }
}