using BLL.Configuration;
using BLL.ServiceInterfaces;
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
            var clientService = provider.GetService<IClientService>();

            var allClients = clientService.GetAll().ToList();
            if (allClients.Count != 0)
            {
                for (int i = 0; i < allClients.Count; i++)
                {
                    Console.WriteLine(allClients[i].Login + " " + allClients[i].Name);
                }
            }
            else
            {
                Console.WriteLine("Нет клиентов");
            }
            Console.WriteLine();

            // сервис для работы с сотрудниками
            var roleService = provider.GetService<IRoleService>();
            var roles = roleService.GetAll().ToList();
            for (int i = 0; i < roles.Count; i++)
            {
                Console.WriteLine($"id:{roles[i].Id};name:{roles[i].Name}");
            }

            // аунтефикация
            // 
            /// 
            /// идем в таблицу clietns с логином 
            /// идем в таблицу workers с логином 

            // авторизация
            /// если нашли в workers то можем получить роль соррудника
            ///
            /// dto c сервиса Auth (у Worker есть поле RoleId) 
            /// по RoleId в roleService.Get(roleId) -> string -> роль залогинившегося сотрудника
            

            //Console.WriteLine(postgresConnection);
        }
    }
}
