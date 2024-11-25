using AutoMapper;
using BLL.DTO;
using BLL.ServiceInterfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    internal class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public void Add(RoleDTO item)
        {
            var role = _mapper.Map<Role>(item);
            _roleRepository.Add(role);
        }

        public void DeleteById(int id)
        {
            var role = _roleRepository.Get(id);
            if (role != null)
            {
                _roleRepository.Delete(role);
            }
        }

        public IEnumerable<RoleDTO> GetAll()
        {
            var roles = _roleRepository.GetAll(); // все роли
            return _mapper.Map<IEnumerable<RoleDTO>>(roles);
        }

        public RoleDTO? GetById(int id)
        {
            var role = _roleRepository.Get(id);
            return role == null ? null : _mapper.Map<RoleDTO>(role);
        }

        public void Update(RoleDTO item)
        {
            var role = _roleRepository.Get(item.Id); // старая роль
            if (role != null)
            {
                _mapper.Map(item, role); // обновляем старую роль новой
                _roleRepository.Update(role); // обновляем в бд
            }
        }
    }
}
