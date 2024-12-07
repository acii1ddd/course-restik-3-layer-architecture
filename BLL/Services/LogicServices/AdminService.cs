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
        private readonly IOrderRepository _orderRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAdminValidatorService _adminValidatorService;
        private IMapper _mapper;

        public AdminService(IWorkerRepository workerRepository, IDishRepository dishRepository, IOrderRepository orderRepository, IRoleRepository roleRepository, IAdminValidatorService adminValidatorService, IMapper mapper)
        {
            _workerRepository = workerRepository;
            _dishRepository = dishRepository;
            _orderRepository = orderRepository;
            _roleRepository = roleRepository;
            _adminValidatorService = adminValidatorService;
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
    }
}
