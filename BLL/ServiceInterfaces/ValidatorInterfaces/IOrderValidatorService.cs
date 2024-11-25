using BLL.DTO;

namespace BLL.ServiceInterfaces.ValidatorsInterfaces
{
    public interface IOrderValidatorService
    {
        public void IsOrderValid(Dictionary<DishDTO, int> selectedDishes, ClientDTO clientId, int tableNumber);
    }
}
