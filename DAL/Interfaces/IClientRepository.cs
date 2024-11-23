using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IClientRepository:IRepository<Client>
    {
        Client? GetByLogin(string login);
    }
}
