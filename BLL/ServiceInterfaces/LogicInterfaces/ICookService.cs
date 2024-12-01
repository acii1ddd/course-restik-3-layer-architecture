using BLL.DTO;

namespace BLL.ServiceInterfaces.LogicInterfaces
{
    public interface ICookService
    {
        public List<OrderDTO> GetAlailableOrders();

        public void TakeAnOrder(int selectedOrder, WorkerDTO cook);

        public List<OrderDTO> GetCurrentOrders(WorkerDTO cook);

        public void MarkOrderAsCooked(int orderNumber);

        public IEnumerable<DishDTO> GetAvailableDishes();

        public List<RecipeDTO> GetDishRecipe(int dishNumber);
    }
}
