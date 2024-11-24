using BLL.DTO;
using BLL.ServiceInterfaces;
using DAL.Interfaces;

namespace BLL.Services
{
    internal class ClientInteractionService : IClientInteractionService
    {
        private readonly IDishRepository _dishRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public ClientInteractionService(IDishRepository dishRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _dishRepository = dishRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public void CreateOrder()
        {
            throw new NotImplementedException();
        }

        public void GetAvailableDishes()
        {
            throw new NotImplementedException();
        }

        public OrderDTO GetOrderStatus(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
