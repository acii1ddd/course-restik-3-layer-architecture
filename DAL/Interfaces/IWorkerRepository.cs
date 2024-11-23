using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IWorkerRepository:IRepository<Worker>
    {
        Worker? GetByLogin(string login);
    }
}
