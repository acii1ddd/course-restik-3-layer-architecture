using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces;
using BLL.ServiceInterfaces.ValidatorsInterfaces;
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
        
        private readonly IOrderValidatorService _orderValidator;

        public ClientInteractionService(IDishRepository dishRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IMapper mapper, IOrderValidatorService orderValidator)
        {
            _dishRepository = dishRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
            _orderValidator = orderValidator;
        }

        public void MakeOrder(Dictionary<DishDTO, int> selectedDishes, ClientDTO clientId, int tableNumber)
        {
            try
            {
                _orderValidator.IsOrderValid(selectedDishes, clientId, tableNumber);
                
                // если заказ валиден для создания - добавляем его в бд
                var order = new Order
                {
                    ClientId = clientId.Id,
                    TableNumber = tableNumber
                };
                _orderRepository.Add(order);

                
                // total_cost в объекте order не обновляется в программе (в бд подсчитывается триггером)


                foreach (var selectedDish in selectedDishes)
                {
                    var dish = _mapper.Map<Dish>(selectedDish.Key); // выбранное блюдо
                    int quantity = selectedDish.Value; // quantity
                    _orderItemRepository.Add(new OrderItem
                    {
                        OrderId = order.Id, // id созданного для клиента заказа
                        DishId = dish.Id,
                        Quantity = quantity,
                        CurrDishPrice = dish.Price
                    });
                }
            }
            catch (Exception)
            {
                throw;
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
