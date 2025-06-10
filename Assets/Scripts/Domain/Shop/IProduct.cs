
namespace Master.Domain.Shop
{
    public interface IProduct
    {
        public ProductState productState { get; }
        public void BuyProduct();

        public void EquipProduct();

        public void UnequipProduct();

        public void OtherProductEquipped();

        public bool IsItPurchasable();
    }
}