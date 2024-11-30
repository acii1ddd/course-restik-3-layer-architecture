using BLL.DTO;

namespace BLL.ServiceInterfaces.LogicInterfaces
{
    public interface ICookService
    {
        public List<OrderDTO> GetAlailableOrders();
    }
}
