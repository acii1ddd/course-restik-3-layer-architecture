using BLL.DTO;
using BLL.ServiceInterfaces.ValidatorsInterfaces;
using DAL.Interfaces;

namespace BLL.Services.Validators
{
    internal class OrderValidatorService : IOrderValidatorService
    {
        private readonly IDishRepository _dishRepository;
        private readonly IClientRepository _clientRepository;

        public OrderValidatorService(IDishRepository dishRepository, IClientRepository clientRepository)
        {
            _dishRepository = dishRepository;
            _clientRepository = clientRepository;
        }

        public void IsOrderValid(Dictionary<DishDTO, int> selectedDishes, ClientDTO client, int tableNumber)
        {
            // если нет такого клиента в базе данных
            if (!_clientRepository.GetAll().ToList().Exists(c => c.Id == client.Id))
            {
                throw new Exception("Клиент с указанным ID не найден.");
            }

            if (tableNumber <= 0)
            {
                throw new Exception("Некорректный номер столика.");
            }

            if (selectedDishes.Count == 0)
            {
                throw new Exception("Список выбранных блюд пуст. Выберите хотя бы одно блюдо.");
            }

            // id'шники доспупных блюд
            var availableDishesIdList = _dishRepository.GetAll().Where(d => d.IsAvailable = true).Select(d => d.Id).ToList();

            foreach (var selectedDish in selectedDishes)
            {
                // все выбранные блюда существуют
                var dishId = selectedDish.Key.Id; // id выбранного блюда
                if (!availableDishesIdList.Contains(dishId))
                {
                    throw new Exception($"Блюдо с ID {dishId} недоступно.");
                }

                // все количества выбранных блюд > 0
                int quantity = selectedDish.Value; // quantity
                if (quantity <= 0)
                {
                    throw new Exception($"Некорректное количество для блюда с ID {dishId}.");
                }
            }
        }
    }
}
