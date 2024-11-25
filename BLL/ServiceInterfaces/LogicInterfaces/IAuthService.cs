using BLL.DTO;
using BLL.ServiceInterfaces.DTOs;

namespace BLL.ServiceInterfaces.LogicInterfaces
{
    public interface IAuthService
    {
        IUserDTO? AuthUser(string login, string password);

        WorkerDTO? CheckWorker(string login, string password);

        ClientDTO? CheckClient(string login, string password);
    }
}
