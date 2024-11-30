using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using course_work.Views;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class CookHandler
    {
        private readonly ICookService _cookService;
        private readonly CookView _cookView;

        public CookHandler(IServiceProvider provider)
        {
            _cookService = provider.GetService<ICookService>() ?? throw new ArgumentNullException();
            _cookView = new CookView();
        }

        public void HandleCook(WorkerDTO worker)
        {
            Console.WriteLine($"\nДобро пожаловать, {worker.FullName}!");
            _cookView.ShowMenu();
            // вызвать mainMenu для показа его функций
            // потом вызвать выбранную функцию из CookService
            //var cookService = provider.GetService<ICookService>();
        }
    }
}
