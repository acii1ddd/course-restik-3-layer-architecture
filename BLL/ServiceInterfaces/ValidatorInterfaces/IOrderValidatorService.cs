using BLL.DTO;

namespace BLL.ServiceInterfaces.ValidatorsInterfaces
{
    public interface IOrderValidatorService
    {
        public void IsOrderValid(Dictionary<DishDTO, int> selectedDishes, ClientDTO clientId, int tableNumber);
        
        public void ValidateClient(ClientDTO client);

        public void ValidateDishAvailability(Dictionary<DishDTO, int> selectedDishes);

        public void ValidateSelectedDishes(Dictionary<DishDTO, int> selectedDishes);

        public void ValidateTableNumber(int tableNumber);

        public DishDTO ValidateDishByNumber(int dishNumber);
    }
}
