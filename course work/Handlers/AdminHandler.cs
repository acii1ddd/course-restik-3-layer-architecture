using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using course_work.Views;
using DAL.Entities;
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
                        ManageWorkers();
                        break;
                    case 2:
                        ManageDishes();
                        break;
                    case 3:
                        //MarkOrderAsCooked(worker);
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

        private bool ShowAllWorkers()
        {
            var workers = _adminService.GetAllWorkers();
            try
            {
                _adminView.PrintWorkers(workers, "Работники ресторана: ");
                return workers.Count() == 0 ? false : true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private void ManageWorkers()
        {
            while (true)
            {
                _adminView.ManageWorkersMenu();
                int choice = Validator.GetValidInteger("Сделайте выбор:");
                switch (choice)
                {
                    case 1:
                        ShowAllWorkers();
                        break;
                    case 2:
                        AddWorker();
                        break;
                    case 3:
                        RemoveWorker();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Выберите корректный номер.");
                        break;
                }
            }
        }

        private void AddWorker()
        {
            Console.WriteLine("\nДобавление нового сотрудника.\n");
            
            // проверка на пустоту строк 
            string roleName = Validator.GetNonEmptyInput("Введите должность сотрудника: ");
            string login = Validator.GetNonEmptyInput("Введите логин сотрудника: ");
            string password = Validator.GetNonEmptyInput("Введите пароль сотрудника: ");
            string phoneNumber = Validator.GetNonEmptyInput("Введите номер телефона: ");

            // Получение даты найма
            DateTime hireDate = _adminView.GetValidHireDate();

            string fullName = Validator.GetNonEmptyInput("Введите имя сотрудника: ");

            try
            {
                _adminService.AddWorker(roleName, login, password, phoneNumber, hireDate, fullName);
                Console.WriteLine("Сотрудник успешно добавлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка добавления работника " + ex.Message);
            }
        }

        private void RemoveWorker()
        {
            string retryInput = "да";
            do
            {
                // заказов нету
                if (ShowAllWorkers())
                {
                    int selectedWorker = HelperUI.GetSelectedNumberWithMessage("Введите номер работника для удаления: ");
                    try
                    {
                        _adminService.DeleteWorker(selectedWorker);
                        Console.WriteLine($"\nРаботник под номером {selectedWorker} успешно удален.");
                        retryInput = "нет";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nОшибка удаления работника. " + ex.Message);
                        retryInput = HelperUI.GetYesOrNoAnswer();
                    }
                }
                else
                {
                    break;
                }
            } while (retryInput == "да");
        }

        private void ManageDishes()
        {
            while (true)
            {
                _adminView.ManageDishesMenu();
                int choice = Validator.GetValidInteger("Сделайте выбор:");
                switch (choice)
                {
                    case 1:
                        ShowAllDishes();
                        break;
                    case 2:
                        //AddWorker();
                        break;
                    case 3:
                        //RemoveWorker();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Выберите корректный номер.");
                        break;
                }
            }
        }

        private void ShowAllDishes()
        {
            Console.WriteLine("Вывод всех блюд");
        }
    }
}
