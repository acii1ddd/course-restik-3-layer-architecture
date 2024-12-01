using BLL.DTO;

namespace BLL.ServiceInterfaces.LogicInterfaces
{
    public interface IWaiterService
    {
        // methods for waiter logic
        public List<OrderDTO> GetAlailableOrders();

        public void TakeAnOrder(int selectedOrder, WorkerDTO waiter);


    }
}
