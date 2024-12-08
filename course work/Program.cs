using BLL.Configuration;
using BLL.ServiceInterfaces.DTOs;
using BLL.ServiceInterfaces.LogicInterfaces;
using course_work.Handlers;
using DAL.Entities;
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
            var services = new ServiceCollection();

            var type = config["DbType:Type"]; // postgres / mongo

            if (type == "postgres")
            {
                var postgresConnection = config.GetConnectionString("PostgresConnection") ?? throw new ArgumentNullException();
                services.ConfigureBLL(postgresConnection, type, "");
            }
            else if (type == "mongo")
            {
                var mongoConnection = config.GetConnectionString("MongoConnection") ?? throw new ArgumentNullException();

                // restaurant запихнуть в appsettings.json
                services.ConfigureBLL(mongoConnection, type, "restaurant");
            }

            // получаем сервис для работы с клиентами
            var provider = services.BuildServiceProvider();

            // -----------------------
            bool isLogin = false;
            while (!isLogin)
            {
                Console.Write("\nВведите логин: ");
                string login = Console.ReadLine();

                Console.Write("Введите пароль: ");
                string password = Console.ReadLine();

                var authService = provider.GetService<IAuthService>();
                var authResult = authService.AuthUser(login, password);

                if (authResult == null)
                {
                    Console.WriteLine("Неверный логин или пароль.");
                    isLogin = false;
                    continue;
                }
                try
                {
                    isLogin = UserHandler.HandleUserRole(authResult, provider);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    throw;
                }
            }
        }
    }
}
