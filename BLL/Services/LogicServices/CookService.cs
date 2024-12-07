using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using BLL.ServiceInterfaces.ValidatorInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services.LogicServices
{
    internal class CookService : ICookService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICookValidatorService _cookValidatorService;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public CookService(IOrderRepository orderRepository, IDishRepository dishRepository, IRecipeRepository recipeRepository, IOrderItemRepository orderItemRepository, ICookValidatorService cookValidatorService, IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _dishRepository = dishRepository;
            _recipeRepository = recipeRepository;
            _orderItemRepository = orderItemRepository;
            _cookValidatorService = cookValidatorService;
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        public List<OrderDTO> GetAlailableOrders()
        {
            var availableOrders = _orderRepository.GetAll().Where(or => or.Status == OrderStatus.InProcessing).ToList();
            if (availableOrders == null || !availableOrders.Any()) // any - нет элементов
            {
                return new List<OrderDTO>(); // Если заказов нет, возвращаем пустой список
            }

            return GetAvailableOrdersWithItems(availableOrders);
        }

        public void TakeAnOrder(int selectedOrder, WorkerDTO cook)
        {
            _cookValidatorService.ValidateTakeOrder(selectedOrder, cook);

            // получаем OrderDTO заказа по номеру, который ввел повар (в том что номер валиден убедились выше)
            var orderToTakeDTO = _cookValidatorService.GetValidOrderByNumber(selectedOrder);
            var orderToTake = _mapper.Map<Order>(orderToTakeDTO); // маппим к обычному order

            orderToTake.Status = OrderStatus.IsCooking;
            orderToTake.CookId = cook.Id;

            _orderRepository.Update(orderToTake);
        }

        public List<OrderDTO> GetCurrentOrders(WorkerDTO cook)
        {
            _cookValidatorService.ValidateCook(cook);

            var currentOrders = _orderRepository
                .GetAll()
                .Where(or => or.Status == OrderStatus.IsCooking) // заказы со статусом "готовится"
                .Where(or => or.CookId == cook.Id) // заказы, которые обрабатывает именно наш повар
                .ToList();

            if (currentOrders == null || !currentOrders.Any()) // any - нет элементов
            {
                return new List<OrderDTO>(); // Если заказов нет, возвращаем пустой список
            }

            return GetAvailableOrdersWithItems(currentOrders);
        }

        public void MarkOrderAsCooked(int orderNumber)
        {
            // получаем OrderDTO по номеру заказа, если он валиден
            var orderToMarkDto = _cookValidatorService.GetValidOrderByNumberToMarkAsCooked(orderNumber);
            var orderToMark = _mapper.Map<Order>(orderToMarkDto);

            orderToMark.Status = OrderStatus.Cooked;
            _orderRepository.Update(orderToMark);
        }

        // список всех доступных блюд
        public IEnumerable<DishDTO> GetAvailableDishes()
        {
            var dishes = _dishRepository.GetAll().Where(d => d.IsAvailable = true);
            return _mapper.Map<IEnumerable<DishDTO>>(dishes);
        }

        public List<RecipeDTO> GetDishRecipe(int dishNumber)
        {
            var dish = _cookValidatorService.ValidateDishByNumber(dishNumber);

            var recipes = _recipeRepository
                .GetAll()
                .Where(r => r.DishId == dish.Id)
                .ToList(); // строки, которые относятся к рецепту нашего блюда

            var ingredients = _ingredientRepository.GetAll().ToList();

            // маппинг рецептов в DTO с прокинутыми в свойства объектами dish и ingredient
            var recipesWithItems = recipes.Select(recipe =>
            {
                // recipeDTO
                var recipeDto = _mapper.Map<RecipeDTO>(recipe);

                // Получаем ингредиент, который связан с нашим рецептом
                var ingredientToRecipe = ingredients.FirstOrDefault(item => item.Id == recipeDto.IngredientId);

                // добавляем ингредиент блюда
                recipeDto.Ingredient = _mapper.Map<IngredientDTO>(ingredientToRecipe);
                recipeDto.Dish = _mapper.Map<DishDTO>(dish);

                return recipeDto;
            }).ToList();

            return recipesWithItems;
        }

        private List<OrderDTO> GetAvailableOrdersWithItems(List<Order> orders)
        {
            var orderItems = _orderItemRepository.GetAll().ToList();
            var dishes = _dishRepository.GetAll().ToList();

            // маппинг заказов в DTO с прокинутыми в свойства объектами
            var currentOrdersWithItems = orders.Select(order =>
            {
                // orderDTO
                var orderDto = _mapper.Map<OrderDTO>(order);

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

            return currentOrdersWithItems;
        }
    }
}
