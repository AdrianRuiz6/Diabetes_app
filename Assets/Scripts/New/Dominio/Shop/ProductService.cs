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

        private Button _productButton;

        private void Awake()
        {
            GameEventsEconomy.OnProductEquipped += OtherProductEquipped;
        }
        private void OnDestroy()
        {
            // Guardado datos producto.
            DataStorage.SaveProduct(_productName, _productState);

            // Cierre de eventos.
            GameEventsEconomy.OnProductEquipped -= OtherProductEquipped;
        }

        void Start()
        {
            // Inicializa el botón con el color que vende.
            _productButton = GetComponent<Button>();
            _sellingColor = _productButton.colors.normalColor;
            _sellingColor.a = 1f;

            // Añadida función cuando se pulsa el botón.
            _productButton.onClick.AddListener(onButtonClick);

            // Cargar datos producto.
            _productState = DataStorage.LoadProduct(_productName);
            if(_productState == ProductState.Equipped)
            {
                GameEventsEconomy.OnProductEquipped?.Invoke(_productName);
            }else if(_productState == ProductState.Purchased)
            {
                GameEventsEconomy.OnProductBought?.Invoke(_productName);
            }
        }

        private void Update()
        {
            if(_productState == ProductState.NotPurchased)
            {
                if (EconomyManager.Instance.GetCoins() < _sellingPrice)
                {
                    GameEventsEconomy.OnNotEnoughMoney(_productName);
                }
                else
                {
                    GameEventsEconomy.OnEnoughMoney(_productName);
                }
            }
        }

        void onButtonClick()
        {
            if (_productState == ProductState.NotPurchased)
            {
                if(EconomyManager.Instance.GetCoins() >= _sellingPrice)
                {
                    UI_confirmationWindowShop.Instance.ShowConfirmationWindow(transform.Find("Product/Dog").gameObject, _sellingPrice.ToString(), BuyProduct);
                }
            }
            else if(_productState == ProductState.Purchased)
            {
                EquipProduct();
            }else if(_productState == ProductState.Equipped)
            {
                _productState = ProductState.Purchased;
                GameEventsEconomy.OnProductBought?.Invoke(_productName);
                GameEventsEconomy.OnProductEquipped?.Invoke("Base");
            }
        }

        private void BuyProduct()
        {
            _productState = ProductState.Purchased;
            EconomyManager.Instance.SubstractCoins(_sellingPrice);
            GameEventsEconomy.OnProductBought?.Invoke(_productName);
        }

        private void EquipProduct()
        {
            _productState = ProductState.Equipped;
            GameEventsEconomy.OnProductEquipped?.Invoke(_productName);
        }

        private void OtherProductEquipped(string productName)
        {
            if(productName != _productName && _productState == ProductState.Equipped)
            {
                _productState = ProductState.Purchased;
            }
        }
    }
}