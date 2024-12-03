using BLL.DTO;

namespace BLL.ServiceInterfaces.ValidatorInterfaces
{
    public interface IWaiterValidatorService
    {
        public OrderDTO GetValidOrderByNumber(int orderNumber);

        public OrderDTO GetValidOrderByNumberToMarkAsDelivered(int orderNumber);

        public OrderDTO GetValidOrderByNumberToAcceptPayment(int orderNumber);

        public void ValidateTakeOrder(int selectedOrder, WorkerDTO cook);

        public void ValidateWaiter(WorkerDTO worker);
    }
}
