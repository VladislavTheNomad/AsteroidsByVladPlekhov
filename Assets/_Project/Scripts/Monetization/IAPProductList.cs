using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace Asteroids
{
    public class IAPProductList : IProductList
    {
        public Dictionary<string, ProductType> Products { get; private set; }

        public IAPProductList()
        {
            Products = new Dictionary<string, ProductType> { { "no_ads", ProductType.NonConsumable } };
        }

        public string GetAdBlockKey()
        {
            foreach (var pair in Products)
            {
                if (pair.Key == "no_ads")
                {
                    return pair.Key;
                }
            }
           
            return null;
        }
    }
}