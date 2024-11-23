using BLL.DTO;
using BLL.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public static class CookHandler
    {
        public static void HandleCook(WorkerDTO worker, IServiceProvider provider)
        {
            Console.WriteLine("Вы повар");
            // вызвать mainMenu для показа его функций
            // потом вызвать выбранную функцию из CookService
            var cookService = provider.GetService<ICookService>();
        }
    }
}
