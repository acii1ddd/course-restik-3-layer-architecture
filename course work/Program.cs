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

            var postgresConnection = config.GetConnectionString("PostgresConnection");
            Console.WriteLine(postgresConnection);
        }
    }
}
