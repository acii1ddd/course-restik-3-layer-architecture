using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using BLL.ServiceInterfaces.ValidatorInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services.LogicServices
{
    internal class WaiterService : IWaiterService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IWaiterValidatorService _waiterValidatorService;
        private readonly IMapper _mapper;

        public WaiterService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IDishRepository dishRepository, IClientRepository clientRepository, IWaiterValidatorService waiterValidatorService, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _dishRepository = dishRepository;
            _clientRepository = clientRepository;
            _waiterValidatorService = waiterValidatorService;
            _mapper = mapper;
        }

        public List<OrderDTO> GetAlailableOrdersToDelivery()
        {
            var availableOrders = _orderRepository.GetAll().Where(or => or.Status == OrderStatus.Cooked).ToList();
            if (availableOrders == null || !availableOrders.Any()) // any - нет элементов
            {
                return new List<OrderDTO>(); // Если заказов нет, возвращаем пустой список
            }

            return GetAvailableOrdersWithItems(availableOrders);
        }

        public void TakeAnOrder(int selectedOrder, WorkerDTO waiter)
        {
            _waiterValidatorService.ValidateTakeOrder(selectedOrder, waiter);

            // получаем OrderDTO заказа по номеру, который ввел повар (в том что номер валиден убедились выше)
            var orderToTakeDTO = _waiterValidatorService.ValidateOrderByNumber(selectedOrder);

            // получаем этот же order из базы данных
            var orderToTake = _orderRepository
                .GetAll()
                .FirstOrDefault(or => or.Id == orderToTakeDTO.Id);

            orderToTake.Status = OrderStatus.InDelivery;
            orderToTake.WaiterId = waiter.Id;

            _orderRepository.Update(orderToTake);
        }

        // delivery/delivered/unpaid
        public List<OrderDTO> GetCurrentOrders(WorkerDTO waiter)
        {
            _waiterValidatorService.ValidateWaiter(waiter);

            var availableOrders = _orderRepository
                .GetAll()
                .Where(or => or.Status == OrderStatus.InDelivery || or.Status == OrderStatus.Delivered)
                .ToList();

            if (availableOrders == null || !availableOrders.Any()) // any - нет элементов
            {
                return new List<OrderDTO>(); // Если заказов нет, возвращаем пустой список
            }

            return GetAvailableOrdersWithItems(availableOrders);
        }

        // delivery/delivered/unpaid
        public List<OrderDTO> GetCurrentUndeliveredOrders(WorkerDTO waiter)
        {
            return GetCurrentOrders(waiter).Where(or => or.Status == OrderStatus.InDelivery).ToList();
        }

        public void MarkOrderAsDelivered(int orderNumber)
        {
            // получаем OrderDTO по номеру заказа, если он валиден
            var orderToMarkDto = _waiterValidatorService.ValidateOrderByNumberToMark(orderNumber);

            // получаем этот же order из базы данных
            var orderToMark = _orderRepository
                .GetAll()
                .FirstOrDefault(or => or.Id == orderToMarkDto.Id);

            orderToMark.Status = OrderStatus.Delivered;
            _orderRepository.Update(orderToMark);
        }

        public List<OrderDTO> GetCurrentUnpaidOrders(WorkerDTO waiter)
        {
            return GetCurrentOrders(waiter).Where(or => or.Status == OrderStatus.Delivered && or.PaymentStatus == PaymentStatus.Unpaid).ToList();
        }

        public void AcceptPaymentForOrder(int selectedOrder, PaymentMethod paymentMethod)
        {
            // получаем OrderDTO по номеру заказа, если он валиден
            var orderToAcceptPaymentDto = _waiterValidatorService.ValidateOrderByNumberToAcceptPayment(selectedOrder);

            // получаем этот же order из базы данных
            var orderToAcceptPayment = _orderRepository
                .GetAll()
                .FirstOrDefault(or => or.Id == orderToAcceptPaymentDto.Id);

            orderToAcceptPayment.PaymentStatus = PaymentStatus.Paid;
            orderToAcceptPayment.Status = OrderStatus.Completed;
            _orderRepository.Update(orderToAcceptPayment);

            // создать объект Payment с PaymentMethod = paymentMethod
            // прокинуть в orderId = orderToAcceptPaymentDto.Id
            // подумать что будет отображаться в Просмотр совершенных заказов у клиента
        }

        private List<OrderDTO> GetAvailableOrdersWithItems(List<Order> availableOrders)
        {
            var orderItems = _orderItemRepository.GetAll().ToList(); // total_cost обновиться триггером из базы данных
            var dishes = _dishRepository.GetAll().ToList();
            var clients = _clientRepository.GetAll().ToList();

            // маппинг заказов в DTO с прокинутыми в свойства объектами
            var availableOrdersWithItems = availableOrders.Select(order =>
            {
                // orderDTO
                var orderDto = _mapper.Map<OrderDTO>(order);

                // Находим клиента, который сделал заказ
                var client = clients.FirstOrDefault(c => c.Id == order.ClientId);
                orderDto.Client = _mapper.Map<ClientDTO>(client); // прокидываем клиента в OrderDTO

                // Получаем позиции заказа для текущего заказа
                var orderItemsForOrder = orderItems.Where(item => item.OrderId == order.Id).ToList();

                // по orders_items ам проходимся
                var orderItemsWithDish = orderItemsForOrder.Select(orderItem =>
                {
                    var orderItemDto = _mapper.Map<OrderItemDTO>(orderItem);

                    // Находим блюдо для позиции заказа
                    var dish = dishes.FirstOrDefault(d => d.Id == orderItem.DishId);
                    orderItemDto.Dish = _mapper.Map<DishDTO>(dish); // добавляем блюдо в DishDTO

                    return orderItemDto;
                }).ToList();

                orderDto.Items = orderItemsWithDish;
                return orderDto;
            }).ToList();

            return availableOrdersWithItems;
        }
    }
}
