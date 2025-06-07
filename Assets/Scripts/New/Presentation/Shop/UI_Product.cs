using Master.Domain.GameEvents;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Master.Domain.Shop;
using Master.Presentation.Sound;

namespace Master.Presentation.Shop
{
    public class UI_Product : MonoBehaviour
    {
        [SerializeField] private string _productName;
        [SerializeField] private int _sellingPrice;
        private Product _product;

        [SerializeField] private GameObject _equippedIcon;
        [SerializeField] private GameObject _boughtIcon;
        [SerializeField] private Color _textColorNotEnoughMoney;
        [SerializeField] private GameObject _priceUI;
        [SerializeField] private TextMeshProUGUI _price_text;

        private Button _productButton;

        private void Awake()
        {
            GameEvents_Shop.OnTotalCoinsUpdated += CheckIsItPurchasable;
            GameEvents_Shop.OnProductEquipped += OnOtherProductEquipped;
        }

        void OnDestroy()
        {
            GameEvents_Shop.OnTotalCoinsUpdated -= CheckIsItPurchasable;
            GameEvents_Shop.OnProductEquipped -= OnOtherProductEquipped;
        }

        void Start()
        {
            _product = new Product(_productName, _sellingPrice);

            _priceUI.SetActive(false);
            _equippedIcon.SetActive(false);
            _boughtIcon.SetActive(false);

            switch (_product.productState)
            {
                case ProductState.NotPurchased:
                    _priceUI.SetActive(true);
                    CheckIsItPurchasable();
                    break;
                case ProductState.Purchased:
                    _boughtIcon.SetActive(true);
                    break;
                case ProductState.Equipped:
                    _equippedIcon.SetActive(true);
                    break;
            }

            // Función cuando se pulsa el botón.
            _productButton.onClick.AddListener(OnProductSelected);
        }

        private void CheckIsItPurchasable(int coins = 0)
        {
            if (_product.productState == ProductState.NotPurchased)
            {
                if (_product.IsItPurchasable())
                {
                    OnEnoughMoney();
                }
                else
                {
                    OnNotEnoughMoney();
                }
            }
        }

        private void OnOtherProductEquipped(string productName)
        {
            if (productName != _productName)
            {
                _priceUI.SetActive(false);
                _equippedIcon.SetActive(false);
                _boughtIcon.SetActive(false);
                if (_product.productState == ProductState.Purchased)
                {
                    _boughtIcon.SetActive(true);
                    _product.OtherProductEquipped();
                }
                else
                {
                    _priceUI.SetActive(true);
                }
            }
        }

        private void OnThisProductEquipped(string productName = null)
        {
            if ((productName == null) || (productName == _productName))
            {
                _priceUI.SetActive(true);
                _equippedIcon.SetActive(false);
                _boughtIcon.SetActive(false);

                _product.EquipProduct();
                SoundManager.Instance.PlaySoundEffect("EquipProduct");
            }
        }

        private void OnProductBought()
        {
            _priceUI.SetActive(false);
            _equippedIcon.SetActive(false);
            _boughtIcon.SetActive(true);

            _product.BuyProduct();
            SoundManager.Instance.PlaySoundEffect("BuyProduct");
        }

        private void OnProductUnequipped()
        {
            _priceUI.SetActive(false);
            _equippedIcon.SetActive(false);
            _boughtIcon.SetActive(true);

            _product.UnequipProduct();
            SoundManager.Instance.PlaySoundEffect("Interaction");
        }

        private void OnEnoughMoney()
        {
            _price_text.color = Color.white;
        }

        private void OnNotEnoughMoney()
        {
            _price_text.color = _textColorNotEnoughMoney;
        }

        void OnProductSelected()
        {
            switch (_product.productState)
            {
                case ProductState.NotPurchased:
                    if (_product.IsItPurchasable())
                    {
                        UI_ConfirmationWindowShop.Instance.ShowConfirmationWindow(transform.Find("Product/Dog").gameObject, _sellingPrice.ToString(), OnProductBought);
                    }
                    else
                    {
                        SoundManager.Instance.PlaySoundEffect("CannotBuyProduct");
                    }
                    break;

                case ProductState.Purchased:
                    OnThisProductEquipped();
                    break;

                case ProductState.Equipped:
                    OnProductUnequipped();
                    break;
            }
        }
    }
}