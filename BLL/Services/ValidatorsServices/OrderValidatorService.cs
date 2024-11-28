using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces.ValidatorsInterfaces;
using DAL.Interfaces;

namespace BLL.Services.Validators
{
    internal class OrderValidatorService : IOrderValidatorService
    {
        private readonly IDishRepository _dishRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public OrderValidatorService(IDishRepository dishRepository, IClientRepository clientRepository, IMapper mapper)
        {
            _dishRepository = dishRepository;
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public void IsOrderValid(Dictionary<DishDTO, int> selectedDishes, ClientDTO client, int tableNumber)
        {
            // если нет такого клиента в базе данных
            ValidateClient(client);

            ValidateTableNumber(tableNumber);

            ValidateSelectedDishes(selectedDishes);

            ValidateDishAvailability(selectedDishes);
        }

        public void ValidateClient(ClientDTO client)
        {
            if (!_clientRepository.GetAll().ToList().Exists(c => c.Id == client.Id))
            {
                throw new Exception("Клиент с указанным ID не найден.");
            }
        }

        public void ValidateDishAvailability(Dictionary<DishDTO, int> selectedDishes)
        {
            // id'шники доспупных блюд
            var availableDishesIdList = _dishRepository.GetAll()
                .Where(d => d.IsAvailable = true)
                .Select(d => d.Id)
                .ToList();

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

        public void ValidateSelectedDishes(Dictionary<DishDTO, int> selectedDishes)
        {
            if (selectedDishes == null || selectedDishes.Count == 0)
            {
                throw new Exception("Список выбранных блюд пуст. Выберите хотя бы одно блюдо.");
            }
        }

        public void ValidateTableNumber(int tableNumber)
        {
            if (tableNumber <= 0)
            {
                throw new Exception("Некорректный номер столика.");
            }
        }

        public DishDTO ValidateDishByNumber(int dishNumber)
        {
            var availableDishes = _dishRepository.GetAll().Where(d => d.IsAvailable = true).ToList();
            if (dishNumber > availableDishes.Count() || dishNumber <= 0)
            {
                throw new ArgumentException($"Блюдо с id {dishNumber} недоступно");
            }
            return _mapper.Map<DishDTO>(availableDishes[dishNumber - 1]);
        }
    }
}
