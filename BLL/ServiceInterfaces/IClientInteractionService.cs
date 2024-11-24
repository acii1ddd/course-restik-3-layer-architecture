using BLL.DTO;

namespace BLL.ServiceInterfaces
{
    public interface IClientInteractionService
    {
        void GetAvailableDishes();

        void CreateOrder();

        OrderDTO GetOrderStatus(int orderId);
    }
}
