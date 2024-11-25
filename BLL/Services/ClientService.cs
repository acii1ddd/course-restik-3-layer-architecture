using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    internal class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository repository, IMapper mapper)
        {
            _clientRepository = repository;
            _mapper = mapper;
        }

        public void Add(ClientDTO item)
        {
            var client = _mapper.Map<Client>(item); // маппим к просто клиенту
            _clientRepository.Add(client);
        }

        public void DeleteById(int id)
        {
            var client = _clientRepository.Get(id);
            if (client != null)
            {
                _clientRepository.Delete(client);
            }
        }

        public IEnumerable<ClientDTO> GetAll()
        {
            var clients = _clientRepository.GetAll(); // взяли всх клиентов
            return _mapper.Map<IEnumerable<ClientDTO>>(clients); // вернули dto-шки
        }

        public ClientDTO? GetById(int id)
        {
            var client = _clientRepository.Get(id); // null либо Client
            return client == null ? null : _mapper.Map<ClientDTO>(client);
        }

        public void Update(ClientDTO item)
        {
            var client = _clientRepository.Get(item.Id); // старый
            if (client != null)
            {
                // обновляем клиента из бд используя новый ClientDTO
                _mapper.Map(item, client);
                _clientRepository.Update(client); // обновляем новым клиентом
            }
        }
    }
}
