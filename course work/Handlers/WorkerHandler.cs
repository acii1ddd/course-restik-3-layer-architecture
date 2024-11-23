using BLL.DTO;
using BLL.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class WorkerHandler
    {



        public void HandleWorker(WorkerDTO worker, IServiceProvider provider)
        {
            var roleService = provider.GetService<IRoleService>();
            string currRole = roleService.GetById(worker.RoleId).Name; // получаем название роли сотрудника
           
            Console.WriteLine($"Добро пожаловать, {worker.FullName}!");
            Console.WriteLine($"Должность: {worker.RoleId}, Логин: {worker.Login} Телефон: {worker.PhoneNumber}, Дата найма: {worker.HireDate}");

            switch (currRole.ToLower())
            {
                case "waiter":
                    WaiterHandler.HandleWaiter(worker, provider);
                    break;
                case "cook":
                    CookHandler.HandleCook(worker, provider);
                    break;
                case "admin":
                    AdminHandler.HandleAdmin(worker, provider);
                    break;
            }
        }
    }
}
