using DAL.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestDAL
{
    public static class Configuration
    {
        public static ServiceProvider ConfigureTestPostgres(out string _testPostgresConnectionString)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // если null (в файле нет строки)
            _testPostgresConnectionString = config.GetConnectionString("TestPostgres") ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");

            var services = new ServiceCollection();
            services.ConfigureDAL(_testPostgresConnectionString, "postgres", "");

            return services.BuildServiceProvider();
        }

        public static ServiceProvider ConfigureTestMongo(out string _testMongoConnectionString, out string _testMongoDbName)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // если null (в файле нет строки)
            _testMongoConnectionString = config.GetConnectionString("TestMongo") ?? throw new InvalidOperationException("Строка подключения для TestMongo не найдена в конфигурации.");


            // получаем имя базы данных
            _testMongoDbName = config.GetSection("MongoSettings:DatabaseName").Value ??
                throw new InvalidOperationException("Имя базы данных для MongoDB не найдено в конфигурации.");

            var services = new ServiceCollection();
            services.ConfigureDAL(_testMongoConnectionString, "mongo", "test_restaurant");

            return services.BuildServiceProvider();
        }
    }
}