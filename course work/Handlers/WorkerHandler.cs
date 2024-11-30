using BLL.DTO;
using BLL.ServiceInterfaces.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class WorkerHandler
    {
        public void HandleWorker(WorkerDTO worker, IServiceProvider provider)
        {
            var roleService = provider.GetService<IRoleService>();
            string currRole = roleService.GetById(worker.RoleId).Name; // получаем название роли сотрудника
            switch (currRole.ToLower())
            {
                case "waiter":
                    WaiterHandler.HandleWaiter(worker, provider);
                    break;
                case "cook":
                    CookHandler cookHandler = new CookHandler(provider);
                    cookHandler.HandleCook(worker);
                    break;
                case "admin":
                    AdminHandler.HandleAdmin(worker, provider);
                    break;
            }
        }
    }
}
