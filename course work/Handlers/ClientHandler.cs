using BLL.DTO;
using BLL.ServiceInterfaces;
using BLL.ServiceInterfaces.DTOs;
using BLL.ServiceInterfaces.ValidatorsInterfaces;
using course_work.Views;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class ClientHandler
    {
        private readonly IClientInteractionService _clientInteractionService;
        private readonly IClientValidatorService _clientValidatorService;
        private readonly ClientView _clientView;

        public ClientHandler(IServiceProvider provider)
        {
            _clientInteractionService = provider.GetService<IClientInteractionService>() ?? throw new ArgumentNullException();
            _clientValidatorService = provider.GetService<IClientValidatorService>() ?? throw new ArgumentNullException();
            _clientView = new ClientView();
        }

        public void HandleClient(ClientDTO client)
        {
            Console.WriteLine($"\nДобро пожаловать, {client.Name}!");
            bool isLeave = false;
            while (true)
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
                        ShowClientOrders(client);
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
                var selectedDishesDict = _clientView.GetSelectedDishes(_clientValidatorService);
                try
                {
                    _clientValidatorService.ValidateSelectedDishes(selectedDishesDict);

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

        public void ShowClientOrders(ClientDTO client)
        {
            try
            {
                var currOrders = _clientInteractionService.GetOrdersForClient(client);
                _clientView.PrintOrders(currOrders);
                //HelperUI.PrintOrders(currOrders, $"\nВаши активные заказы: ", "У вас нет активных заказов.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nОшибка получения списка заказов. " + ex.Message);
                throw;
            }
        }
    }
}
