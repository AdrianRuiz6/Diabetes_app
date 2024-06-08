using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.States;
using Master.Domain.Events;

namespace Master.Domain.Economy
{
    public class ProductService : MonoBehaviour
    {
        [SerializeField] private string _productName;
        [SerializeField] private int _sellingPrice;
        private ProductState _productState;
        private Color _sellingColor;

        private Button _myButton;

        void Start()
        {
            // Inicializa el botón con el color que vende.
            _myButton = GetComponent<Button>();
            _sellingColor = _myButton.colors.normalColor;
            _sellingColor.a = 1f;

            // Añadida función cuando se pulsa el botón.
            _myButton.onClick.AddListener(onButtonClick);

            // Cargar datos producto.
            _productState = DataStorage.LoadProduct(_productName);
            if(_productState == ProductState.Equiped)
            {
                GameEventsEconomy.OnProductEquiped?.Invoke(_sellingColor);
            }
        }

        void onButtonClick()
        {
            if (_productState == ProductState.NotPurchased)
            {
                if(EconomyManager.Instance.GetCoins() >= _sellingPrice)
                {
                    BuyProduct();
                }
                else
                {
                    Debug.Log("No tienes suficientes monedas.");
                }
            }
            else if(_productState == ProductState.Purchased)
            {
                EquipProduct();
            }
        }

        private void BuyProduct()
        {
            _productState = ProductState.Purchased;
            EconomyManager.Instance.SubstractCoins(_sellingPrice);
            GameEventsEconomy.OnProductBought?.Invoke(true);
        }

        private void EquipProduct()
        {
            _productState = ProductState.Equiped;
            GameEventsEconomy.OnProductEquiped?.Invoke(_sellingColor);
        }

        private void OnDestroy()
        {
            // Guardado datos producto.
            DataStorage.SaveProduct_____(_productName, _productState);
        }
    }
}