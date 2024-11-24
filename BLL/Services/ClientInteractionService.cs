using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    internal class ClientInteractionService : IClientInteractionService
    {
        private readonly IDishRepository _dishRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public ClientInteractionService(IDishRepository dishRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IMapper mapper)
        {
            _dishRepository = dishRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public void MakeOrder(Dictionary<DishDTO, int> selectedDishes, ClientDTO clientId, int tableNumber)
        {
            if (selectedDishes == null || selectedDishes.Count == 0 || clientId.Id <= 0)
            {
                return;
            }
            // создаем заказ для клиента (без блюд пока что)
            var order = new Order
            {
                ClientId = clientId.Id,
                TableNumber = tableNumber
            };
            _orderRepository.Add(order);

            foreach (var selectedDish in selectedDishes)
            {
                var dish = _mapper.Map<Dish>(selectedDish.Key); // выбранное блюдо
                int quantity = selectedDish.Value; // quantity

                if (quantity <= 0)
                {
                    continue;
                }
                if (!_dishRepository.GetAll().ToList().Any(d => d.Id == dish.Id)) // есть ли блюдо с нужным id в бд
                {
                    continue;
                }

                _orderItemRepository.Add(new OrderItem
                {
                    OrderId = order.Id, // id созданного для клиента заказа
                    DishId = dish.Id,
                    Quantity = quantity,
                    CurrDishPrice = dish.Price
                });
            }
        }

        // список всех доступных блюд
        public IEnumerable<DishDTO> GetAvailableDishes()
        {
            var dishes = _dishRepository.GetAll().Where(d => d.IsAvailable = true);
            return _mapper.Map<IEnumerable<DishDTO>>(dishes);
        }

        public OrderDTO GetOrderStatus(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
