using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public static class AdminHandler
    {
        public static void HandleAdmin(WorkerDTO worker, IServiceProvider provider)
        {
            Console.WriteLine("Вы администратор");
            // вызвать mainMenu для показа его функций
            // потом вызвать выбранную функцию из AdminService
            var adminService = provider.GetService<IAdminService>();
        }
    }
}
