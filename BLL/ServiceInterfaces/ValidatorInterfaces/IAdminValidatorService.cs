using BLL.DTO;
using DAL.Entities;

namespace BLL.ServiceInterfaces.ValidatorInterfaces
{
    public interface IAdminValidatorService
    {
        public void ValidateWorkerData(string role, string login, string password, string phoneNumber, DateTime hireDate, string fullName);

        public WorkerRole GetRoleNameDescription(string roleName);

        public WorkerDTO GetValidWorkerByNumber(int workerNumber);

        void ValidateGetOrdersForPeriod(DateTime startDate, DateTime endDate);
    }
}
