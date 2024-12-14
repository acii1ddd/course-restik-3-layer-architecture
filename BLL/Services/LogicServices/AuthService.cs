using BLL.DTO;
using DAL.Interfaces;
using AutoMapper;
using BLL.ServiceInterfaces.LogicInterfaces;
using BLL.ServiceInterfaces.DTOs;

namespace BLL.Services.LogicServices
{
    internal class AuthService : IAuthService
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public AuthService(IWorkerRepository workerRepository, IClientRepository clientRepository, IMapper mapper)
        {
            _workerRepository = workerRepository;
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        // наверх нужно dto
        public IUserDTO? AuthUser(string login, string password)
        {
            // нашли сотрудника
            var worker = CheckWorker(login, password);
            if (worker != null)
            {
                return worker;
            }

            // нашли клиента
            var client = CheckClient(login, password);
            if (client != null)
            {
                return client;
            }
            return null;
        }

        // если есть клиент с таким логином и паролем возвр его dto, если нету - null
        private ClientDTO? CheckClient(string login, string password)
        {
            var client = _clientRepository.GetByLogin(login);
            if (client != null && client.Password == password)
            {
                return _mapper.Map<ClientDTO>(client);
            }
            return null;
        }

        // если есть сотрудник с таким логином и паролем возвр его dto, если нету - null
        private WorkerDTO? CheckWorker(string login, string password)
        {
            var worker = _workerRepository.GetByLogin(login);
            if (worker != null && worker.Password == password)
            {
                return _mapper.Map<WorkerDTO>(worker);
            }
            return null;
        }
    }
}
