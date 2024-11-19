using AutoMapper;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    public class ClientService : IClientRepository
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository repository, IMapper mapper)
        {
            _clientRepository = repository;
            _mapper = mapper;
        }

        public void Add(Client entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Client entity)
        {
            throw new NotImplementedException();
        }

        public Client? Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Client> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Client entity)
        {
            throw new NotImplementedException();
        }
    }
}
