using System.Collections.Generic;
using UnityEngine.Purchasing;
using Zenject;

namespace Asteroids
{
    public class ProductList
    {
        public Dictionary<string, ProductType> Products { get; private set; }

        [Inject]
        public ProductList()
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