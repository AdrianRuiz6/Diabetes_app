using Master.Domain.GameEvents;
using UnityEngine;
using TMPro;

namespace Master.Presentation.Shop
{
    public class UI_Product : MonoBehaviour
    {
        [SerializeField] private string _productName;
        [SerializeField] private GameObject _equippedIcon;
        [SerializeField] private GameObject _boughtIcon;
        [SerializeField] private Color _textColorNotEnoughMoney;
        [SerializeField] private GameObject _priceUI;
        [SerializeField] private TextMeshProUGUI _price_text;

        private bool _isBought;

        private void Awake()
        {
            GameEvents_Shop.OnProductEquipped += OnProductEquipped;
            GameEvents_Shop.OnProductBought += OnProductBought;
            GameEvents_Shop.OnNotEnoughMoney += OnNotEnoughMoney;
            GameEvents_Shop.OnEnoughMoney += OnEnoughMoney;
        }

        void OnDestroy()
        {
            GameEvents_Shop.OnProductEquipped -= OnProductEquipped;
            GameEvents_Shop.OnProductBought -= OnProductBought;
            GameEvents_Shop.OnNotEnoughMoney -= OnNotEnoughMoney;
            GameEvents_Shop.OnEnoughMoney -= OnEnoughMoney;
        }

        void Start()
        {
            _priceUI.SetActive(true);
            _equippedIcon.SetActive(false);
            _boughtIcon.SetActive(false);

            _isBought = false;
        }

        private void OnProductEquipped(string productName)
        {
            if (productName == _productName)
            {
                _priceUI.SetActive(false);
                _boughtIcon.SetActive(false);
                _isBought = true;

                _equippedIcon.SetActive(true);
            }
            else
            {
                _equippedIcon.SetActive(false);
                if (_isBought == true)
                {
                    OnProductBought(_productName);
                }
            }
        }

        private void OnProductBought(string productName)
        {
            if (productName == _productName)
            {
                _priceUI.SetActive(false);
                _equippedIcon.SetActive(false);
                _isBought = true;

                _boughtIcon.SetActive(true);
            }
        }

        private void OnNotEnoughMoney(string productName)
        {
            if (productName == _productName)
            {
                _price_text.color = _textColorNotEnoughMoney;
            }
        }

        private void OnEnoughMoney(string productName)
        {
            if (productName == _productName)
            {
                _price_text.color = Color.white;
            }
        }
    }
}