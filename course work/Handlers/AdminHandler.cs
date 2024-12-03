using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using course_work.Views;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class AdminHandler
    {
        private readonly IAdminService _adminService;
        private readonly AdminView _adminView;

        public AdminHandler(IServiceProvider provider)
        {
            _adminService = provider.GetService<IAdminService>() ?? throw new ArgumentNullException();
            _adminView = new AdminView();
        }

        public void HandleAdmin(WorkerDTO worker)
        {
            Console.WriteLine($"\nДобро пожаловать, {worker.FullName}!");
            bool isLeave = false;
            while (true)
            {

                _adminView.ShowMenu(); // Menu с функционалом

                // выбор клиента
                int choice = Validator.GetValidInteger("Введите корректное значение:");
                switch (choice)
                {
                    case 1:
                        ShowAllWorkers();
                        break;
                    case 2:
                        //TakeAnOrder(worker);
                        break;
                    case 3:
                        //ShowCurrentOrders(worker);
                        break;
                    case 4:
                        //MarkOrderAsCooked(worker);
                        break;
                    case 5:
                        //ViewDishRecipe();
                        break;
                    case 0:
                        isLeave = true;
                        Console.WriteLine("Выход");
                        break; // switch
                    default:
                        Console.WriteLine("Выберите корректный номер.");
                        break;
                }
                if (isLeave)
                {
                    break; // while
                }
            }
        }

        private void ShowAllWorkers()
        {
            Console.WriteLine("Все работники!!");
        }
    }
}
