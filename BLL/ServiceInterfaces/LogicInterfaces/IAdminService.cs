using BLL.DTO;
using BLL.Services.LogicServices;
using DAL.Entities;

namespace BLL.ServiceInterfaces.LogicInterfaces
{
    public interface IAdminService
    {
        // methods for admin logic
        public List<WorkerDTO> GetAllWorkers();

        public void AddWorker(string roleName, string login, string password, string phoneNumber, DateTime hireDate, string fullName);

        void DeleteWorker(int selectedWorker);

        public List<OrderDTO> GetOrdersForPeriod(DateTime startDate, DateTime endDate);

        public List<DishDTO> GetTheMostPopularDishes();
    }
}
