using BLL.DTO;
using BLL.ServiceInterfaces.DTOs;

namespace course_work.Handlers
{
    public static class UserHandler
    {
        public static bool HandleUserRole(IUserDTO authResult, IServiceProvider provider)
        {
            if (authResult is ClientDTO client)
            {
                var clientHandler = new ClientHandler(provider);
                clientHandler.HandleClient(client);
                return false; // клиент вышел
            }
            else if (authResult is WorkerDTO worker)
            {
                var workerHandler = new WorkerHandler();
                workerHandler.HandleWorker(worker, provider);
                return false; // сотрудник вышел
            }
            throw new Exception("Ошибка обработки пользователя");
        }
    }
}
