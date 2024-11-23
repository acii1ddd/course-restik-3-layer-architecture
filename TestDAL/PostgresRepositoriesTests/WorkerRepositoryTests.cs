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

        private void ClearTableRoles()
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "TRUNCATE TABLE roles RESTART IDENTITY CASCADE";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void ShureRoleExists(int id, string roleName)
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "INSERT INTO roles (id, name) VALUES (@id, @roleName) ON CONFLICT DO NOTHING";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@roleName", roleName);
                    command.ExecuteNonQuery();
                }
            }
        }


        // добавление worrker без узказания даты (current date в базе данных)
        [Fact]
        public void AddWorker_ShouldAddWorker()
        {
            ClearTable();
            // если уже есть роль с id = 1, то ничего не делаем и будем использовать ее, если ее нет - создали
            ShureRoleExists(1, "TestRole1");

            // Id = 0
            var worker = new Worker
            {
                RoleId = 1,
                Login = "test_add_worker",
                Password = "test_add_pass",
                PhoneNumber = "test_add_phone",
                FullName = "test_add_name"
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
                        Assert.Equal(DateTime.Now.Date, reader["hire_date"]); // текущая дата дается в бд
                        Assert.Equal(worker.FullName, reader["full_name"]);
                    }
                }
            }
            ClearTable();
            ClearTableRoles();
        }

        [Fact]
        public void GetWorker_ShouldReturnWorker()
        {
            // очистка табллицы clients
            ClearTable();
            ShureRoleExists(1, "TestRole1");

            var worker = new Worker
            {
                RoleId = 1,
                Login = "test_get_worker",
                Password = "test_get_pass",
                PhoneNumber = "test_get_phone",
                FullName = "test_get_name"
            };

            var receivedWorker = new Worker();

            // Act
            receivedWorker = _workerRepository.Get(worker.Id);
            // Assert
            Assert.Null(receivedWorker); // нет клиента с таким Id - получаем null

            _workerRepository.Add(worker);
            // Act
            receivedWorker = _workerRepository.Get(worker.Id);
            // Assert
            Assert.NotNull(receivedWorker);
            Assert.Equal(worker.RoleId, receivedWorker.RoleId);
            Assert.Equal(worker.Login, receivedWorker.Login);
            Assert.Equal(worker.Password, receivedWorker.Password);
            Assert.Equal(worker.PhoneNumber, receivedWorker.PhoneNumber);
            Assert.Equal(DateTime.Now.Date, receivedWorker.HireDate); // текущая дата из базы данных
            Assert.Equal(worker.FullName, receivedWorker.FullName);
            ClearTable();
            ClearTableRoles();
        }

        [Fact]
        public void DeleteWorker_ShouldDeleteWorker()
        {
            ClearTable();
            ShureRoleExists(1, "TestRole1");
            var worker = new Worker
            {
                RoleId = 1,
                Login = "test_del_worker",
                Password = "test_del_pass",
                PhoneNumber = "test_del_phone",
                FullName = "test_del_name"
            };

            _workerRepository.Add(worker);

            // Act
            _workerRepository.Delete(worker);

            // Assert
            var deletedClient = _workerRepository.Get(worker.Id);
            Assert.Null(deletedClient);
            ClearTable();
            ClearTableRoles();
        }

        [Fact]
        public void GetAllWorkers_ShouldReturnAllWorkers()
        {
            ClearTable();
            ShureRoleExists(1, "TestRole1");
            var worker1 = new Worker
            {
                RoleId = 1,
                Login = "test_getAll_worker",
                Password = "test_getAll_pass",
                PhoneNumber = "test_getAll_phone",
                FullName = "test_getAll_name"
            };

            // пустой лист, если ничего нету в таблице
            var workers = new List<Worker>();

            // Act
            workers = _workerRepository.GetAll().ToList();

            // Assert
            Assert.Empty(workers);

            // не пустой лист
            _workerRepository.Add(worker1);

            // Act
            workers = _workerRepository.GetAll().ToList();

            // Assert
            Assert.Single(workers); // одна штука

            Assert.Contains(workers, w => w.Login == worker1.Login);
            Assert.Contains(workers, w => w.Password == worker1.Password);
            Assert.Contains(workers, w => w.PhoneNumber == worker1.PhoneNumber);
            ClearTable();
            ClearTableRoles();
        }

        [Fact]
        public void UpdateWorker_ShouldUpdateExistingWorker()
        {
            ClearTable();
            ShureRoleExists(1, "TestRole1");
            var worker = new Worker
            {
                RoleId = 1,
                Login = "test_del_worker",
                Password = "test_del_pass",
                PhoneNumber = "test_del_phone",
                FullName = "test_del_name"
            };
            _workerRepository.Add(worker);

            // поменяли поля для обновления старого сотрудника
            ShureRoleExists(2, "TestRole2");
            worker.RoleId = 2;
            worker.Login = "updated_login";
            worker.Password = "updated_pass";
            worker.PhoneNumber = "updated_phone";
            worker.FullName = "updated_name";

            // Act
            _workerRepository.Update(worker);

            // Assert
            var updatedWorker = _workerRepository.Get(worker.Id);
            Assert.NotNull(updatedWorker);
            Assert.Equal(updatedWorker.RoleId, worker.RoleId);
            Assert.Equal(updatedWorker.Login, worker.Login);
            Assert.Equal(updatedWorker.Password, worker.Password);
            Assert.Equal(updatedWorker.PhoneNumber, worker.PhoneNumber);
            Assert.Equal(updatedWorker.FullName, worker.FullName);
            ClearTable();
            ClearTableRoles();
        }

        [Fact]
        public void GetWorkerByLogin_ReturnsWorkerOrNULL()
        {
            ClearTable();
            ShureRoleExists(1, "TestRole1");
            var worker = new Worker
            {
                RoleId = 1,
                Login = "test_getById_worker",
                Password = "test_getById_pass",
                PhoneNumber = "test_getById_phone",
                FullName = "test_getById_name"
            };

            // нет сотрудника с таким логином
            // Act 1
            var receivedWorker = _workerRepository.GetByLogin(worker.Login);

            // Assert
            Assert.Null(receivedWorker);

            // есть сотрудник с таким логином
            _workerRepository.Add(worker);

            // Act 2
            receivedWorker = _workerRepository.GetByLogin(worker.Login);

            // Assert
            Assert.NotNull(receivedWorker);
            Assert.Equal(receivedWorker.Login, worker.Login);
            Assert.Equal(receivedWorker.Password, worker.Password);
            Assert.Equal(receivedWorker.PhoneNumber, worker.PhoneNumber);
            Assert.Equal(receivedWorker.HireDate, DateTime.Now.Date);
            Assert.Equal(receivedWorker.FullName, worker.FullName);
            
            ClearTable();
            ClearTableRoles();
        }
    }
}