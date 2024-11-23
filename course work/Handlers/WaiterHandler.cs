using BLL.DTO;
using Microsoft.Extensions.DependencyInjection;
using BLL.ServiceInterfaces;

namespace course_work.Handlers
{
    public static class WaiterHandler
    {
        public static void HandleWaiter(WorkerDTO worker, IServiceProvider provider)
        {
            Console.WriteLine("Вы официант");
            // вызвать mainMenu для показа его функций
            // потом вызвать выбранную функцию из WaiterService
            var waiterService = provider.GetService<IWaiterService>();
        }
    }
}
