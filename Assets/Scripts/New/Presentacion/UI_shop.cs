using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Master.Domain.Events;

namespace Master.UI.Economy
{
    public class UI_shop : MonoBehaviour
    {
        public static UI_shop Instance;

        private GameObject _coinsAmountObject;
        private TMP_Text _coinsAmountText;

        private GameObject _characterObj;
        private Image _characterImage;

        private void Awake()
        {
            // Singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }else if(Instance != this)
            {
                Destroy(gameObject);
            }

            //Events
            GameEventsEconomy.OnCoinsUpdated += OnCoinsUpdated;
            GameEventsEconomy.OnProductEquiped += OnProductEquiped;
            GameEventsEconomy.OnProductBought += OnProductBought;
        }

        void OnDestroy()
        {
            // Events
            GameEventsEconomy.OnCoinsUpdated -= OnCoinsUpdated;
            GameEventsEconomy.OnProductEquiped -= OnProductEquiped;
            GameEventsEconomy.OnProductBought -= OnProductBought;
        }

        private void Start()
        {
            // Inicialización de CoinsAmountText.
            _coinsAmountObject = GameObject.Find("CoinsAmount_TMP");
            _coinsAmountText = _coinsAmountObject.GetComponent<TMP_Text>();

            // Inicialización de Imagen del personaje.
            _characterObj = GameObject.Find("Character_Img");
            _characterImage = _characterObj.GetComponent<Image>();
        }

        private void OnCoinsUpdated(int coins)
        {
            _coinsAmountText.text = coins.ToString();
        }

        private void OnProductEquiped(Color sellingColor)
        {
            _characterImage.color = sellingColor;
        }

        private void OnProductBought(bool isAcquired)
        {

        }
    }
}