using Master.Domain.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.Economy
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

