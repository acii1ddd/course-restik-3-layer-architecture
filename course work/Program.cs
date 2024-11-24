using BLL.Configuration;
using BLL.ServiceInterfaces;
using course_work.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace course_work
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // путь, где ConfigurationBuilder будет искать файлы конфигурации
                                                              // optional: false - файл appsettings.json обязателен reloadOnChange:true - обновлять конфигурацию при изменении файла.
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var postgresConnection = config.GetConnectionString("PostgresConnection") ?? throw new ArgumentNullException();
            var services = new ServiceCollection();
            services.ConfigureBLL(postgresConnection);

            // получаем сервис для работы с клиентами
            var provider = services.BuildServiceProvider();

            //// сервис для работы с сотрудниками
            //var roleService = provider.GetService<IRoleService>();
            //var roles = roleService.GetAll().ToList();
            //for (int i = 0; i < roles.Count; i++)
            //{
            //    Console.WriteLine($"id:{roles[i].Id};name:{roles[i].Name}");
            //}

            //var workerService = provider.GetService<IWorkerService>();
            //var workers = workerService.GetAll().ToList();
            //Console.WriteLine();
            //if (workers.Count != 0)
            //{
            //    for (int i = 0; i < workers.Count; i++)
            //    {
            //        Console.WriteLine($"id:{workers[i].Id};role_id:{workers[i].RoleId};login:{workers[i].Login}" +
            //            $";phone:{workers[i].PhoneNumber};hire_date:{workers[i].HireDate};full_name:{workers[i].FullName}");
            //    }
            //}

            Console.Write("Введите логин: ");
            string login = Console.ReadLine();

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            var authService = provider.GetService<IAuthService>();
            var authResult = authService.AuthUser(login, password);

            if (authResult == null)
            {
                Console.WriteLine("Неверный логин или пароль.");
                return;
            }
            // вошел клиент
            //else if (authResult is ClientDTO client)
            //{
            //    Console.WriteLine($"Добро пожаловать, {client.Name}!");
            //    Console.WriteLine($"Вы клиент!!!");
            //    Console.WriteLine($"Ваш логин: {client.Login}!");

            //    // mainMenu для показа его функций
            //    Console.WriteLine($"\n\nВаше меню:");

            //    // потом вызвать выбранную функцию из ClientService
            //}
            //// вошел работник
            //else if (authResult is WorkerDTO worker)
            //{
            //    var roleService = provider.GetService<IRoleService>();
            //    string currRole = roleService.GetById(worker.RoleId).Name;
            //    if (currRole != null)
            //    {
            //        Console.WriteLine($"Добро пожаловать, {worker.FullName}!");
            //        Console.WriteLine($"Должность: {worker.RoleId}, Логин: {worker.Login} Телефон: {worker.PhoneNumber}, Дата найма: {worker.HireDate}");
            //        if (currRole == "waiter")
            //        {
            //            Console.WriteLine("Вы официант");
            //            // вызвать mainMenu для показа его функций
            //            // потом вызвать выбранную функцию из WaiterService
            //        }
            //        if (currRole == "cook")
            //        {
            //            Console.WriteLine("Вы повар");
            //            // вызвать mainMenu для показа его функций
            //            // потом вызвать выбранную функцию из CookService
            //        }
            //        if (currRole == "admin")
            //        {
            //            Console.WriteLine("Вы администратор");
            //            // вызвать mainMenu для показа его функций
            //            // потом вызвать выбранную функцию из AdminService
            //        }
            //    }
            //}

            //Console.WriteLine(postgresConnection);

            UserHandler.HandleUserRole(authResult, provider);
        }
    }
}
