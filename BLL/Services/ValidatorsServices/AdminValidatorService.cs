using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces.ValidatorInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services.ValidatorsServices
{
    internal class AdminValidatorService : IAdminValidatorService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IMapper _mapper;

        public AdminValidatorService(IRoleRepository roleRepository, IWorkerRepository workerRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _workerRepository = workerRepository;
            _mapper = mapper;
        }

        public void ValidateWorkerData(string roleName, string login, string password, string phoneNumber, DateTime hireDate, string fullName)
        {
            // существует ли роль с таким именем (enum значение - так хранится в бд)
            if (!_roleRepository.GetAll().Any(r => r.Name == GetRoleNameDescription(roleName).ToString()))
                throw new ArgumentException($"Должность '{roleName}' не существует.");
            
            if (hireDate < DateTime.Now.Date)
                throw new ArgumentException($"Дата не должна быть меньше текущей.");
        }

        // преобразовать русскую роль к английской (которая в бд)
        public WorkerRole GetRoleNameDescription(string roleName)
        {
            switch (roleName.ToLower())
            {
                case "официант":
                    return WorkerRole.waiter;
                case "повар":
                    return WorkerRole.cook;
                default:
                    throw new ArgumentException($"Должность '{roleName}' не существует.");
            }
        }

        public WorkerDTO GetValidWorkerByNumber(int workerNumber)
        {
            // все сотрудники
            var availableWorkers = _mapper.Map<List<WorkerDTO>>(_workerRepository.GetAll().ToList());

            // прокинуть role в dto шки
            foreach (var workerDto in availableWorkers)
            {
                workerDto.Role = _mapper.Map<RoleDTO>(_roleRepository.Get(workerDto.RoleId));
            }
            // все кроме admin, так как admin не удаляет сам себя
            return GetValidWorkerByNumberFromList(availableWorkers.Where(w => w.Role.Name != "admin").ToList(), workerNumber);
        }

        private WorkerDTO GetValidWorkerByNumberFromList(List<WorkerDTO> workers, int workerNumber)
        {
            if (workers == null || !workers.Any())
            {
                throw new InvalidOperationException("Нет доступных заказов для выполнения.");
            }

            if (workerNumber > workers.Count() || workerNumber <= 0)
            {
                throw new ArgumentException($"Сотрудник с id {workerNumber} недоступен");
            }
            return _mapper.Map<WorkerDTO>(workers[workerNumber - 1]);
        }

        public void ValidateGetOrdersForPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Начальная дата не может быть позже конечной даты.");
            }

            if (startDate > DateTime.Now || endDate > DateTime.Now)
            {
                throw new ArgumentException("Дата не может быть в будущем.");
            }
        }
    }
}
