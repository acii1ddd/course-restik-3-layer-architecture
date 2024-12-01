using BLL.DTO;

namespace BLL.ServiceInterfaces.ValidatorInterfaces
{
    public interface ICookValidatorService
    {

        public OrderDTO ValidateOrderByNumber(int orderNumber);

        public OrderDTO ValidateOrderByNumberToMark(int orderNumber);

        public void ValidateCook(WorkerDTO worker);

        public DishDTO ValidateDishByNumber(int dishNumber);
        void ValidateTakeOrder(int selectedOrder, WorkerDTO cook);
    }
}
