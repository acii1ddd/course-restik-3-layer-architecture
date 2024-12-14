using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services.ArchiveHandlers
{
    internal class PostgresArchiveHandler : IArchiveHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public PostgresArchiveHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public void ArchiveOrderWithItems(OrderDTO order)
        {
            // триггер в бд postgres добавляет удаляемый заказ в orders_archive
            // триггер в бд postgres добавляет позиции удаляемого заказа в orders_items_archive

            _orderRepository.Delete(_mapper.Map<Order>(order)); // срабатывает триггер в бд - прокидывает в архивную таблицу
        }
    }
}
