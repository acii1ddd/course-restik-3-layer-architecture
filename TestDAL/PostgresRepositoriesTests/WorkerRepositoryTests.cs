using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Xml.Linq;

namespace TestDAL.PostgresRepositoriesTests
{
    public class WorkerRepositoryTests
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly string _testPostgresConnectionString;

        public WorkerRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTest(out _testPostgresConnectionString);
            _workerRepository = serviceProvider.GetService<IWorkerRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearTable()
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "TRUNCATE TABLE workers RESTART IDENTITY CASCADE";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        [Fact]
        public void AddRole_ShouldAddRole()
        {
            ClearTable();
            // Id = 0
            var worker = new Worker
            {
                RoleId = 1,
                Login = "test_add_worker_login",
                Password = "test_add_worker_password",
                PhoneNumber = "test_add_worker_phone_number",
                HireDate = DateOnly.Parse("2024-11-23"),
                FullName = "test_add_worker_full_name"
            };

            try
            {
                // Вызов метода добавления клиента, меняется Id в client
                _workerRepository.Add(worker);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении клиента: {ex.Message}");
            }

            // Assert
            Assert.NotEqual(0, worker.Id);

            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM workers WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", worker.Id); // autoinc в бд

                    using (var reader = command.ExecuteReader())
                    {
                        // true если reader вернет true, исключение с сообщением "Role not found in database." - если false
                        Assert.True(reader.Read(), "Role not found in database."); // проверка reader на true/false
                        Assert.Equal(worker.RoleId, reader["role_id"]);
                        Assert.Equal(worker.Login, reader["login"]);
                        Assert.Equal(worker.Password, reader["password"]);
                        Assert.Equal(worker.PhoneNumber, reader["phone_number"]);
                        Assert.Equal(worker.HireDate, reader["hire_date"]);
                        Assert.Equal(worker.FullName, reader["full_name"]);
                    }
                }
            }
        }
    }
}