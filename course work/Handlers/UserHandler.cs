using BLL.DTO;
using BLL.ServiceInterfaces.DTOs;

namespace course_work.Handlers
{
    public static class UserHandler
    {
        public static void HandleUserRole(IUserDTO authResult, IServiceProvider provider)
        {
            if (authResult is ClientDTO client)
            {
                var clientHandler = new ClientHandler(provider);
                clientHandler.HandleClient(client);
            }
            else if (authResult is WorkerDTO worker)
            {
                var workerHandler = new WorkerHandler();
                workerHandler.HandleWorker(worker, provider);
            }
        }
    }
}
