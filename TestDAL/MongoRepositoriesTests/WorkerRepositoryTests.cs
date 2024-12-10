using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestDAL.MongoRepositoriesTests
{
    public class WorkerRepositoryTests
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IRoleRepository _roleRepository;

        private readonly IMongoCollection<Role> _roleCollection;
        private readonly IMongoCollection<Worker> _workerCollection;

        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public WorkerRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);

            _roleCollection = database.GetCollection<Role>("roles");
            _workerCollection = database.GetCollection<Worker>("workers");

            _workerRepository = serviceProvider.GetService<IWorkerRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _roleRepository = serviceProvider.GetService<IRoleRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearCollections()
        {
            _roleCollection.DeleteMany(Builders<Role>.Filter.Empty);
            _workerCollection.DeleteMany(Builders<Worker>.Filter.Empty);
        }

        private Role ShureRoleExists(string roleName)
        {
            var role = new Role
            {
                Name = roleName
            };
            _roleRepository.Add(role);

            return role;
        }

        // добавление worrker без узказания даты (current date в базе данных)
        [Fact]
        public void AddWorker()
        {
            ClearCollections();
            // если уже есть роль с id = 1, то ничего не делаем и будем использовать ее, если ее нет - создали
            ShureRoleExists("TestRole1");

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

            var retrievedWorker = _workerCollection.Find(r => r.Id == worker.Id).FirstOrDefault();

            // Assert
            Assert.Equal(1, worker.Id);
            Assert.Equal(worker.RoleId, retrievedWorker.RoleId);
            Assert.Equal(worker.Login, retrievedWorker.Login);
            Assert.Equal(worker.Password, retrievedWorker.Password);
            Assert.Equal(worker.PhoneNumber, retrievedWorker.PhoneNumber);
            Assert.Equal(DateTime.UtcNow.Date, retrievedWorker.HireDate.Date); // текущая дата в бд в utc
            Assert.Equal(worker.FullName, retrievedWorker.FullName);
            
            ClearCollections();
        }

        [Fact]
        public void GetWorker()
        {
            ClearCollections();
            ShureRoleExists("TestRole1");

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
            Assert.Equal(worker.HireDate.ToString("yyyy-MM-dd HH:mm:ss"), receivedWorker.HireDate.ToString("yyyy-MM-dd HH:mm:ss")); // текущая дата из базы данных
            Assert.Equal(worker.FullName, receivedWorker.FullName);
            ClearCollections();
        }

        [Fact]
        public void DeleteWorker()
        {
            ClearCollections();
            ShureRoleExists("TestRole1");
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
            ClearCollections();
        }

        [Fact]
        public void GetAllWorkers()
        {
            ClearCollections();
            ShureRoleExists("TestRole1");
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
            ClearCollections();
        }

        [Fact]
        public void UpdateWorker()
        {
            ClearCollections();
            ShureRoleExists("TestRole1");
            var worker = new Worker
            {
                RoleId = 1,
                Login = "test_update_worker",
                Password = "test_update_pass",
                PhoneNumber = "test_update_phone",
                FullName = "test_update_name"
            };
            _workerRepository.Add(worker);

            // поменяли поля для обновления старого сотрудника
            ShureRoleExists("TestRole2");
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
            Assert.Equal(updatedWorker.HireDate.ToString("yyyy-MM-dd HH:mm:ss"), worker.HireDate.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.Equal(updatedWorker.FullName, worker.FullName);
            ClearCollections();
        }

        [Fact]
        public void GetWorkerByLogin_ReturnsWorkerOrNULL()
        {
            ClearCollections();
            ShureRoleExists("TestRole1");
            var worker = new Worker
            {
                RoleId = 1,
                Login = "test_getByLog_worker",
                Password = "test_getByLog_pass",
                PhoneNumber = "test_getByLog_phone",
                FullName = "test_getByLog_name"
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
            Assert.Equal(receivedWorker.HireDate.ToString("yyyy-MM-dd HH:mm:ss"), worker.HireDate.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.Equal(receivedWorker.FullName, worker.FullName);

            ClearCollections();
        }
    }
}