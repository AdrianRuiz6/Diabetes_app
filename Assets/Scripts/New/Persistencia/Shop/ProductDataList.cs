using Master.Domain.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Persistence.Shop
{
    [System.Serializable]
    public class ProductDataList
    {
        public List<ProductData> products = new List<ProductData>();
    }
}