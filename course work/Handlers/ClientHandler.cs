using BLL.DTO;
using BLL.ServiceInterfaces;
using course_work.Views;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class ClientHandler
    {
        private readonly IClientService _clientService;

        public ClientHandler(IServiceProvider provider)
        {
            _clientService = provider.GetService<IClientService>() ?? throw new ArgumentNullException();
        }

        public void HandleClient(ClientDTO client)
        {
            Console.WriteLine($"Добро пожаловать, {client.Name}!");

            // mainMenu для показа его функций
            var view = new ClientView();
            view.ShowMenu();
            // выбор клиент
            var input = (Console.ReadLine());

            if (int.TryParse(input, out int choice)) // input парсится к int
            {
                switch (choice)
                {
                    case 1:
                        // Просмотр меню блюд
                        //_clientService.
                        break;
                    case 2:
                        //Совершение заказа
                        //_clientService.
                        break;
                    case 3:
                        //Просмотр статуса заказа
                        //_clientService.
                        break;
                    default:
                        Console.WriteLine("Выберите корректно.");
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
