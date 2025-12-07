using UnityEngine.SceneManagement;
using Zenject;

namespace Asteroids
{
    public class MenuUiModel
    {
        private IAPService _iapService;
        private IAPProductList _productList;

        [Inject]
        public void Construct(IAPService iapService, IAPProductList productList)
        {
            _iapService = iapService;
            _productList = productList;
        }

        public void StartGame()
        {
            SceneManager.LoadScene("MainScene");
        }

        internal void BuyAdBlock()
        {
            _iapService.BuyProduct(_productList.GetAdBlockKey());
        }
    }
}
