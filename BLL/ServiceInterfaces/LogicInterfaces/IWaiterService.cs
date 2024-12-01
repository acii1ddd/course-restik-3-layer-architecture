using BLL.DTO;
using DAL.Entities;

namespace BLL.ServiceInterfaces.LogicInterfaces
{
    public interface IWaiterService
    {
        // methods for waiter logic
        public List<OrderDTO> GetAlailableOrdersToDelivery();

        public void TakeAnOrder(int selectedOrder, WorkerDTO waiter);

        public List<OrderDTO> GetCurrentOrders(WorkerDTO waiter);

        public void MarkOrderAsDelivered(int orderNumber);

        public List<OrderDTO> GetCurrentUnpaidOrders(WorkerDTO waiter);

        void AcceptPaymentForOrder(int selectedOrder, PaymentMethod paymentMethod);

        public List<OrderDTO> GetCurrentUndeliveredOrders(WorkerDTO waiter);
    }
}
