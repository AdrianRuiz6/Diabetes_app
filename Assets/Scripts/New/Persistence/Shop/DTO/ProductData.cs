using Master.Domain.Shop;

namespace Master.Persistence.Shop
{
    [System.Serializable]
    public class ProductData
    {
        public string ProductName;
        public ProductState ProductState;

        public ProductData()
        {
            ProductState = ProductState.NotPurchased;
        }
        public ProductData(string productName, ProductState productState)
        {
            this.ProductName = productName;
            this.ProductState = productState;
        }
    }
}