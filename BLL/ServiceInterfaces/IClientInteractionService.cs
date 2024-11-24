using BLL.DTO;

namespace BLL.ServiceInterfaces
{
    public interface IClientInteractionService
    {
        IEnumerable<DishDTO> GetAvailableDishes();

        void MakeOrder(Dictionary<DishDTO, int> selectedDishes, ClientDTO clientId, int tableNumber);

        OrderDTO GetOrderStatus(int orderId);
    }
}
