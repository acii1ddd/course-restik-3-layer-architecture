using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces.ValidatorInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services.ValidatorsServices
{
    internal class WaiterValidatorService : IWaiterValidatorService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IMapper _mapper;

        public WaiterValidatorService(IOrderRepository orderRepository, IWorkerRepository workerRepository, IDishRepository dishRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _workerRepository = workerRepository;
            _dishRepository = dishRepository;
            _mapper = mapper;
        }

        public OrderDTO ValidateOrderByNumber(int orderNumber)
        {
            var availableOrders = _orderRepository.GetAll()
                .Where(or => or.Status == OrderStatus.Cooked)
                .ToList();

            if (availableOrders == null || !availableOrders.Any())
            {
                throw new InvalidOperationException("Нет доступных заказов для выполнения.");
            }

            if (orderNumber > availableOrders.Count() || orderNumber <= 0)
            {
                throw new ArgumentException($"Заказ с id {orderNumber} недоступен");
            }
            return _mapper.Map<OrderDTO>(availableOrders[orderNumber - 1]);
        }

        public OrderDTO ValidateOrderByNumberToMark(int orderNumber)
        {
            var availableOrders = _orderRepository.GetAll()
                .Where(or => or.Status == OrderStatus.InDelivery)
                .ToList();

            if (availableOrders == null || !availableOrders.Any())
            {
                throw new InvalidOperationException("Нет доступных заказов для выполнения.");
            }

            if (orderNumber > availableOrders.Count() || orderNumber <= 0)
            {
                throw new ArgumentException($"Заказ с id {orderNumber} недоступен");
            }
            return _mapper.Map<OrderDTO>(availableOrders[orderNumber - 1]);
        }

        public OrderDTO ValidateOrderByNumberToAcceptPayment(int orderNumber)
        {
            var availableOrders = _orderRepository.GetAll()
                .Where(or => or.Status == OrderStatus.Delivered && or.PaymentStatus == PaymentStatus.Unpaid)
                .ToList();

            if (availableOrders == null || !availableOrders.Any())
            {
                throw new InvalidOperationException("Нет доступных заказов для приятия оплаты.");
            }

            if (orderNumber > availableOrders.Count() || orderNumber <= 0)
            {
                throw new ArgumentException($"Заказ с id {orderNumber} недоступен");
            }

            var orderToAcceptPayment = availableOrders[orderNumber - 1];
            if (orderToAcceptPayment.Status != OrderStatus.Delivered && orderToAcceptPayment.PaymentStatus != PaymentStatus.Paid)
            {
                throw new InvalidOperationException($"Заказ с Id {orderToAcceptPayment.Id} не может быть оплачен, так как его статус.");
            }

            return _mapper.Map<OrderDTO>(orderToAcceptPayment);
        }

        public void ValidateWaiter(WorkerDTO worker)
        {
            // есть ли такой повар в бд
            var cookExists = _workerRepository.GetAll().Any(wor => wor.Id == worker.Id);
            if (!cookExists)
            {
                throw new InvalidOperationException($"Повар с Id {worker.Id} не найден.");
            }
        }

        public void ValidateTakeOrder(int selectedOrder, WorkerDTO cook)
        {
            ValidateOrderByNumber(selectedOrder);
            ValidateWaiter(cook);
        }
    }
}
