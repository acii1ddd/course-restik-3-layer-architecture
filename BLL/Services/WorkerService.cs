using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    internal class WorkerService : IWorkerService //!!!! INTERNAL
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IMapper _mapper;

        public WorkerService(IWorkerRepository workerRepository, IMapper mapper)
        {
            _workerRepository = workerRepository;
            _mapper = mapper;
        }

        public void Add(WorkerDTO item)
        {
            var worker = _mapper.Map<Worker>(item);
            _workerRepository.Add(worker);
        }

        public void DeleteById(int id)
        {
            var worker = _workerRepository.Get(id);
            if (worker != null)
            {
                _workerRepository.Delete(worker);
            }
        }

        public IEnumerable<WorkerDTO> GetAll()
        {
            var workers = _workerRepository.GetAll(); // все роли
            return _mapper.Map<IEnumerable<WorkerDTO>>(workers);
        }

        public WorkerDTO? GetById(int id)
        {
            var worker = _workerRepository.Get(id);
            return worker == null ? null : _mapper.Map<WorkerDTO>(worker);
        }

        public void Update(WorkerDTO item)
        {
            var worker = _workerRepository.Get(item.Id); // старая роль
            if (worker != null)
            {
                _mapper.Map(item, worker); // обновляем старого сотрудника новым
                _workerRepository.Update(worker); // обновляем в бд
            }
        }
    }
}
