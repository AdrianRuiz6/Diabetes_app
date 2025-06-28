using Master.Domain.GameEvents;
using UnityEngine.UI;
using UnityEngine;
using Master.Domain.Shop;
using System.Collections.Generic;
using Master.Infrastructure;

namespace Master.Presentation.Shop
{
    public class UI_Character : MonoBehaviour
    {
        private Image _characterImage;

        [SerializeField] private Sprite _baseColor;
        [SerializeField] private Sprite _yellowColor;
        [SerializeField] private Sprite _blueColor;
        [SerializeField] private Sprite _purpleColor;
        [SerializeField] private Sprite _orangeColor;
        [SerializeField] private Sprite _redColor;
        [SerializeField] private Sprite _pinkColor;
        [SerializeField] private Sprite _lightPinkColor;
        [SerializeField] private Sprite _greenColor;

        private IEconomyManager _economyManager;

        private void Awake()
        {
            GameEvents_Shop.OnProductEquipped += OnProductEquiped;
        }

        void OnDestroy()
        {
            GameEvents_Shop.OnProductEquipped -= OnProductEquiped;
        }

        private void Start()
        {
            _economyManager = ServiceLocator.Instance.GetService<IEconomyManager>();

            // Inicialización de Imagen del personaje.
            _characterImage = GetComponent<Image>();
            _characterImage.sprite = _baseColor;

            Dictionary<string, ProductState> allProducts = _economyManager.LoadAllProducts();
            if (allProducts != null && allProducts.Count > 0)
            {
                foreach (var product in allProducts)
                {
                    if (product.Value == ProductState.Equipped)
                    {
                        OnProductEquiped(product.Key);
                    }
                }
            }
        }

        private void OnProductEquiped(string productName)
        {
            switch (productName)
            {
                case "Base":
                    _characterImage.sprite = _baseColor;
                    break;
                case "Yellow":
                    _characterImage.sprite = _yellowColor;
                    break;
                case "Blue":
                    _characterImage.sprite = _blueColor;
                    break;
                case "Purple":
                    _characterImage.sprite = _purpleColor;
                    break;
                case "Orange":
                    _characterImage.sprite = _orangeColor;
                    break;
                case "Red":
                    _characterImage.sprite = _redColor;
                    break;
                case "Pink":
                    _characterImage.sprite = _pinkColor;
                    break;
                case "LightPink":
                    _characterImage.sprite = _lightPinkColor;
                    break;
                case "Green":
                    _characterImage.sprite = _greenColor;
                    break;
                default:
                    break;
            }
        }
    }
}