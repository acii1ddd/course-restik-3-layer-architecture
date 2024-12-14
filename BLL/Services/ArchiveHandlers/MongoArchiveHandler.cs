using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.ArchiveHandlers
{
    internal class MongoArchiveHandler : IArchiveHandler
    {
        private readonly IOrderArchiveRepository _orderArchiveRepository;
        private readonly IOrderItemArchiveRepository _orderItemArchiveRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public MongoArchiveHandler(IOrderArchiveRepository orderArchiveRepository, IOrderItemArchiveRepository orderItemArchiveRepository, IMapper mapper, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _orderArchiveRepository = orderArchiveRepository;
            _orderItemArchiveRepository = orderItemArchiveRepository;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        // добавление заказа в архивную коллекцию
        public void ArchiveOrderWithItems(OrderDTO order)
        {
            _orderArchiveRepository.Add(_mapper.Map<Order>(order));
            ArchiveOrderItems(order);
            DeleteOrderAndItems(order);
        }

        // добавление элементов заказа в архивную коллекцию
        private void ArchiveOrderItems(OrderDTO order)
        {
            foreach (var orderItem in order.Items)
            {
                _orderItemArchiveRepository.Add(_mapper.Map<OrderItem>(orderItem));
            }
        }

        private void DeleteOrderAndItems(OrderDTO order)
        {
            // удаление всех позициий заказа
            foreach (var orderItem in order.Items)
            {
                _orderItemRepository.Delete(_mapper.Map<OrderItem>(orderItem));
            }

            // удаление саммого заказа
            _orderRepository.Delete(_mapper.Map<Order>(order));
        }
    }
}
