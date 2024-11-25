using BLL.DTO;

namespace BLL.ServiceInterfaces
{
    public interface IClientInteractionService
    {
        IEnumerable<DishDTO> GetAvailableDishes();

        // returns bool
        // isMake - true if order is make correctly otherwise - false
        void MakeOrder(Dictionary<DishDTO, int> selectedDishes, ClientDTO clientId, int tableNumber);

        OrderDTO GetOrderStatus(int orderId);
    }
}
