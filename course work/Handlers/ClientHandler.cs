using BLL.DTO;
using BLL.ServiceInterfaces;
using course_work.Views;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class ClientHandler
    {
        private readonly IClientService _clientService;
        private readonly IClientInteractionService _clientInteractionService;
        private readonly ClientView _clientView;

        public ClientHandler(IServiceProvider provider)
        {
            _clientService = provider.GetService<IClientService>() ?? throw new ArgumentNullException();
            _clientInteractionService = provider.GetService<IClientInteractionService>() ?? throw new ArgumentNullException();
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
                        ShowAvailableDishes();

                        // словарь для выбранных блюд
                        var selectedDishes = new Dictionary<DishDTO, int>();
                        
                        while (true)
                        {
                            Console.Write("\nВведите номер блюда (или нажмите Enter для завершения): ");
                            var input = Console.ReadLine();

                            if (string.IsNullOrEmpty(input))
                            {
                                break; // закончил ввод
                            }

                            if (!int.TryParse(input, out int dishId))
                            {
                                Console.WriteLine("Некорректный номер блюда. Повторите ввод.");
                                continue;
                            }

                            var availableDishes = _clientInteractionService.GetAvailableDishes().ToList();
                            dishId--; // индекс массива с 0
                            if (dishId >= availableDishes.Count || dishId < 0)
                            {
                                Console.Write("\nНет такого блюда в меню.");
                                continue;
                            }

                            var dish = availableDishes.ToList()[dishId];

                            Console.Write("Введите количество блюда: ");
                            int quantity = Validator.GetValidInteger("Введите корректное количество:");

                            if (quantity <= 0)
                            {
                                Console.WriteLine("Количество должно быть больше 0. Повторите ввод.");
                                continue;
                            }

                            if (selectedDishes.ContainsKey(dish))
                            {
                                selectedDishes[dish] += quantity; // увелич кол-во, если блюдо уже выбрано
                            }
                            else
                            {
                                selectedDishes[dish] = quantity;
                            }
                            Console.WriteLine($"Добавлено: {dish.Name}, кол-во - {quantity}");
                        }


                        // выбрано хотя бы 1 блюдо
                        if (selectedDishes.Count > 0)
                        {
                            Console.Write("\nВведите номер столика для заказа: ");
                            int tableNumber = Validator.GetValidInteger("Введите корректный номер:");
                            MakeOrder(selectedDishes, client, tableNumber); // Передаем заказ в сервис
                        }
                        else
                        {
                            Console.WriteLine("Вы не выбрали ни одного блюда. Заказ не был создан.");
                            Console.WriteLine("Хотите попробовать сделать заказ снова? (да/нет):");
                            var retryInput = Console.ReadLine()?.ToLower();

                            if (retryInput != "да" && retryInput != "yes")
                            {
                                break; // Если клиент не хочет повторять заказ
                            }
                        }
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

            // потом вызвать выбранную функцию из ClientService
            
            // logic
            //var allClients = clientService.GetAll().ToList();
            //if (allClients.Count != 0)
            //{
            //    for (int i = 0; i < allClients.Count; i++)
            //    {
            //        Console.WriteLine(allClients[i].Login + " " + allClients[i].Name);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Нет клиентов");
            //}
            //Console.WriteLine();
        }

        private void ShowAvailableDishes()
        {
            var dishes = _clientInteractionService.GetAvailableDishes().ToList();
            _clientView.PrintDishes(dishes);
        }

        private void MakeOrder(Dictionary<DishDTO, int> selectedDishes, ClientDTO client, int tableNumber)
        {
            //_clientInteractionService.MakeOrder(selectedDishes, client, tableNumber);
            Console.WriteLine("Заказ совершен");
        }
    }
}
