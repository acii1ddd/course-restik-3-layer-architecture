using DAL.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestDAL
{
    public static class Configuration
    {
        public static ServiceProvider ConfigureTest(out string _testPostgresConnectionString)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // если null (в файле нет строки)
            _testPostgresConnectionString = config.GetConnectionString("TestPostgres") ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");

            var services = new ServiceCollection();
            services.ConfigureDAL(_testPostgresConnectionString, "postgres");

            return services.BuildServiceProvider();
        }
    }
}