using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using DAL.Interfaces;

namespace BLL.Services
{
    internal class CookService : ICookService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IRecipeRepository _recipeRepository;

        public CookService(IOrderRepository orderRepository, IDishRepository dishRepository, IRecipeRepository recipeRepository)
        {
            _orderRepository = orderRepository;
            _dishRepository = dishRepository;
            _recipeRepository = recipeRepository;
        }

        public List<OrderDTO> GetAlailableOrders()
        {
            throw new NotImplementedException();
        }
    }
}
