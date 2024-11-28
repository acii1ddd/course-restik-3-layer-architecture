using BLL.DTO;
using BLL.ServiceInterfaces;
using BLL.ServiceInterfaces.ValidatorsInterfaces;
using course_work.Views;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class ClientHandler
    {
        private readonly IClientService _clientService;
        private readonly IClientInteractionService _clientInteractionService;
        private readonly IOrderValidatorService _orderValidatorService;
        private readonly ClientView _clientView;

        public ClientHandler(IServiceProvider provider)
        {
            _clientService = provider.GetService<IClientService>() ?? throw new ArgumentNullException();
            _clientInteractionService = provider.GetService<IClientInteractionService>() ?? throw new ArgumentNullException();
            _orderValidatorService = provider.GetService<IOrderValidatorService>() ?? throw new ArgumentNullException();
            _clientView = new ClientView();
        }

        public void HandleClient(ClientDTO client)
        {
            Console.WriteLine($"\nДобро пожаловать, {client.Name}!");
            bool isLeave = false;
            while(true)
            {
                _clientView.ShowMenu(); // Menu с функционалом

                // выбор клиента
                int choice = Validator.GetValidInteger("Введите корректное значение:");
                switch (choice)
                {
                    case 1:
                        ShowAvailableDishes();
                        break;
                    case 2:
                        ProcessOrder(client);
                        break;
                    case 3:
                        Console.WriteLine("Просмотр статуса заказа");
                        //_clientService.
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

        private void ShowAvailableDishes()
        {
            var dishes = _clientInteractionService.GetAvailableDishes().ToList();
            _clientView.PrintDishes(dishes);
        }

        private void ProcessOrder(ClientDTO client)
        {
            string retryInput = "да";
            do
            {
                ShowAvailableDishes();
                var selectedDishesDict = _clientView.GetSelectedDishes(_orderValidatorService);
                try
                {
                    _orderValidatorService.ValidateSelectedDishes(selectedDishesDict);

                    // выбрано хотя бы 1 блюдо
                    int tableNumber = Validator.GetValidInteger(
                            "Введите номер столика для заказа: ",
                            "Введите корректный номер столика.\n",
                            dishId => dishId > 0
                    );
                    _clientInteractionService.MakeOrder(selectedDishesDict, client, tableNumber); // Передаем заказ в сервис
                    Console.WriteLine("\nВаш заказ успешно совершен. Ожидайте.");
                    retryInput = "нет";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nОшибка совершения заказа. " + ex.Message);
                    retryInput = _clientView.GetYesOrNoAnswer();
                }
            } while (retryInput == "да");
        }
    }
}
