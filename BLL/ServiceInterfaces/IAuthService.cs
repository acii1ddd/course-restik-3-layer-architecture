using BLL.DTO;

namespace BLL.ServiceInterfaces
{
    public interface IAuthService
    {
        IUserDTO? AuthUser(string login, string password);

        WorkerDTO? CheckWorker(string login, string password);

        ClientDTO? CheckClient(string login, string password);
    }
}
