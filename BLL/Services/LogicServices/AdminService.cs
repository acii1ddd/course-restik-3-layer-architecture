using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using BLL.ServiceInterfaces.ValidatorInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services.LogicServices
{
    internal class AdminService : IAdminService
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IOrderArchiveRepository _orderArchiveRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAdminValidatorService _adminValidatorService;
        private readonly IOrderItemArchiveRepository _orderItemArchiveRepository;
        private readonly IClientRepository _clientRepository;
        private IMapper _mapper;

        public AdminService(IWorkerRepository workerRepository, IDishRepository dishRepository, IOrderArchiveRepository orderArchiveRepository, IRoleRepository roleRepository, IAdminValidatorService adminValidatorService, IOrderItemArchiveRepository orderItemArchiveRepository, IClientRepository clientRepository, IMapper mapper)
        {
            _workerRepository = workerRepository;
            _dishRepository = dishRepository;
            _orderArchiveRepository = orderArchiveRepository;
            _roleRepository = roleRepository;
            _adminValidatorService = adminValidatorService;
            _orderItemArchiveRepository = orderItemArchiveRepository;
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public List<WorkerDTO> GetAllWorkers()
        {
            // все workers в dto шках
            var workerDtos = _mapper.Map<List<WorkerDTO>>(_workerRepository.GetAll().ToList());
            foreach (var workerDto in workerDtos)
            {
                workerDto.Role = _mapper.Map<RoleDTO>(_roleRepository.Get(workerDto.RoleId));
            }
            // все кроме администраторов будут показываться для самого администратора
            return workerDtos.Where(w => w.Role.Name != "admin").ToList();
        }

        public void AddWorker(string roleName, string login, string password, string phoneNumber, DateTime hireDate, string fullName)
        {
            _adminValidatorService.ValidateWorkerData(roleName, login, password, phoneNumber, hireDate, fullName);

            Worker workerToAdd = new Worker
            {
                RoleId = _roleRepository.GetAll().FirstOrDefault(r => r.Name == _adminValidatorService.GetRoleNameDescription(roleName).ToString()).Id,
                Login = login,
                Password = password,
                PhoneNumber = phoneNumber,
                HireDate = hireDate,
                FullName = fullName
            };

            _workerRepository.Add(workerToAdd);
        }

        public void DeleteWorker(int selectedWorker)
        {
            // получаем WorkerDTO по номеру, если он валиден
            var workerToDeleteDto = _adminValidatorService.GetValidWorkerByNumber(selectedWorker);
            var workerToDelete = _mapper.Map<Worker>(workerToDeleteDto);

            _workerRepository.Delete(workerToDelete);
        }

        // пустой List<OrderDTO> если нет подходящих заказов, если есть - лист с этими заказами
        public List<OrderDTO> GetOrdersForPeriod(DateTime startDate, DateTime endDate)
        {
            // если даты валидны - получаем список заказов за период, иначе exception
            _adminValidatorService.ValidateGetOrdersForPeriod(startDate, endDate);
            
            // если даты равны - получаем заказы просто за эту дату
            if (startDate == endDate)
            {
                return GetOrdersWithItems(
                    _orderArchiveRepository
                        .GetAll()
                        .Where(or => or.Date.Date == startDate.Date)
                        .ToList()
                );
            }
            // если даты не равны - получаем заказы за период
            else
            {
                return GetOrdersWithItems(
                    _orderArchiveRepository
                        .GetAll()
                        .Where(or => or.Date.Date >= startDate.Date && or.Date.Date <= endDate)
                        .ToList()
                );
            }
        }

        // этот метод и в adminService и в waiterService и в cookService и в ClientInteractionService
        private List<OrderDTO> GetOrdersWithItems(List<Order> orders)
        {
            if (orders == null || !orders.Any()) // any - нет элементов
            {
                return new List<OrderDTO>(); // Если заказов нет, возвращаем пустой список
            }

            var orderItems = _orderItemArchiveRepository.GetAll().ToList(); // total_cost обновиться триггером из базы данных
            var dishes = _dishRepository.GetAll().ToList();
            var clients = _clientRepository.GetAll().ToList();

            // маппинг заказов в DTO с прокинутыми в свойства объектами
            var availableOrdersWithItems = orders.Select(order =>
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

        // топ 3 самых популярных блюд по общему количеству во всех заказах
        public List<DishDTO> GetTheMostPopularDishes()
        {
            // позиции блюд всех вообще заказов
            var orderItemsArchive = _orderItemArchiveRepository.GetAll().ToList();
            var dishes = _dishRepository.GetAll().ToList();

            // группируем позиции заказа по блюду и подсчитываем их количество
            var popularDishes = orderItemsArchive
                .GroupBy(item => item.DishId) // группируем по dish_id
                .Select(group => new // для каждой группы берем dish_id и кол-во в этой группе
                {
                    DishId = group.Key,
                    TotalQuantity = group.Sum(item => item.Quantity) // общее количество блюда DishId во всех заказах
                })
                .OrderByDescending(d => d.TotalQuantity) // сортируем по убываию количеств в группе
                .Take(3) // самые популярные блюда
                .ToList();

            // маппинг популярных блюд в DTO
            var popularDishDtos = popularDishes // DishId, Count в popularDishes
                .Select(pd =>
                {
                    var dish = dishes.FirstOrDefault(d => d.Id == pd.DishId);
                    return _mapper.Map<DishDTO>(dish);
                })
                .ToList();

            return popularDishDtos;
        }
    }
}
