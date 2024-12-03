using BLL.DTO;

namespace BLL.ServiceInterfaces.ValidatorInterfaces
{
    public interface ICookValidatorService
    {
        public OrderDTO GetValidOrderByNumber(int orderNumber);

        public OrderDTO GetValidOrderByNumberToMarkAsCooked(int orderNumber);

        public void ValidateCook(WorkerDTO worker);

        public DishDTO ValidateDishByNumber(int dishNumber);

        void ValidateTakeOrder(int selectedOrder, WorkerDTO cook);
    }
}
