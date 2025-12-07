namespace Asteroids
{
    public interface IStoreService
    {
        bool CheckProductStatus(string id);
        void BuyProduct(string id);
    }
}