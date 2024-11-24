using BLL.DTO;
using BLL.ServiceInterfaces;
using course_work.Views;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class ClientHandler
    {
        private readonly IClientService _clientService;
        private readonly ClientView _clientView;

        public ClientHandler(IServiceProvider provider)
        {
            _clientService = provider.GetService<IClientService>() ?? throw new ArgumentNullException();
            _clientView = new ClientView();
        }

        public void HandleClient(ClientDTO client)
        {
            Console.WriteLine($"\nДобро пожаловать, {client.Name}!");

            // Menu с функционалом
            _clientView.ShowMenu();
            
            // выбор клиента
            if (int.TryParse(Console.ReadLine(), out int choice)) // input парсится к int
            {
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Просмотр меню блюд");
                        //_clientService.
                        break;
                    case 2:
                        Console.Write("Введите номер столика для заказа: ");
                        Console.ReadLine();
                        // проверка на число

                        //_clientService.
                        break;
                    case 3:
                        Console.WriteLine("Просмотр статуса заказа");
                        //_clientService.
                        break;
                    default:
                        Console.WriteLine("Выберите корректный номер.");
                        break;
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
    }
}
