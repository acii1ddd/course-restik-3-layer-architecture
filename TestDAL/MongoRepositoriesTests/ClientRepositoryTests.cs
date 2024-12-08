using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestDAL.MongoRepositoriesTests
{
    public class ClientRepositoryTests
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMongoCollection<Client> _collection;
        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public ClientRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);
            _collection = database.GetCollection<Client>("clients");

            _clientRepository = serviceProvider.GetService<IClientRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        // очистка коллекции перед каждым тестом
        private void ClearCollection()
        {
            _collection.DeleteMany(Builders<Client>.Filter.Empty);
        }

        [Fact]
        public void AddClient_ShouldAddClient()
        {
            ClearCollection();
            // Id = 0
            var client = new Client
            {
                Login = "test_add_login",
                Password = "test_add_password",
                Name = "test_add_name",
            };

            try
            {
                _clientRepository.Add(client);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении клиента: {ex.Message}");
            }

            Assert.Equal(1, client.Id); // Проверка, что Id не null

            var retrievedClient = _collection.Find(c => c.Id == client.Id).FirstOrDefault(); // Поиск клиента асинхронно

            Assert.NotNull(retrievedClient);
            Assert.Equal(client.Login, retrievedClient.Login);
            Assert.Equal(client.Password, retrievedClient.Password);
            Assert.Equal(client.Name, retrievedClient.Name);

            ClearCollection();
        }

        [Fact]
        public void GetClient_ShouldReturnClientById()
        {
            ClearCollection();

            var client = new Client()
            {
                Login = "test_get_login",
                Password = "test_get_password",
                Name = "test_get_name"
            };

            var receivedClient = new Client();

            // Act
            receivedClient = _clientRepository.Get(client.Id);
            // Assert
            Assert.Null(receivedClient); // нет клиента с таким Id - получаем null

            _clientRepository.Add(client);

            // Act
            receivedClient = _clientRepository.Get(client.Id);
            // Assert
            Assert.NotNull(receivedClient);
            Assert.Equal(client.Login, receivedClient.Login);
            Assert.Equal(client.Password, receivedClient.Password);
            Assert.Equal(client.Name, receivedClient.Name);
            ClearCollection();
        }

        [Fact]
        public void DeleteClient_ShouldDeleteClient()
        {
            ClearCollection();
            var client = new Client()
            {
                Login = "test_delete_login",
                Password = "test_delete_password",
                Name = "test_delete_name"
            };

            _clientRepository.Add(client);

            // Act
            _clientRepository.Delete(client);

            // Assert
            var deletedClient = _clientRepository.Get(client.Id);
            Assert.Null(deletedClient);
            ClearCollection();
        }

        [Fact]
        public void GetAllClients_ShouldReturnAllClients()
        {
            ClearCollection();
            var client1 = new Client()
            {
                Login = "test_getAll_login1",
                Password = "test_getAll_password1",
                Name = "test_getAll_name1"
            };

            var client2 = new Client()
            {
                Login = "test_getAll_login2",
                Password = "test_getAll_password2",
                Name = "test_getAll_name2"
            };

            // пустой лист, если ничего нету в таблице
            var clients = new List<Client>();

            // Act
            clients = _clientRepository.GetAll().ToList();

            // Assert
            Assert.Empty(clients);

            // не пустой лист
            _clientRepository.Add(client1);
            _clientRepository.Add(client2);

            // Act
            clients = _clientRepository.GetAll().ToList();

            // Assert
            Assert.Equal(2, clients.Count);

            Assert.Contains(clients, c => c.Login == client1.Login);
            Assert.Contains(clients, c => c.Login == client2.Login);
            ClearCollection();
        }

        [Fact]
        public void UpdateClient_ShouldUpdateExistingClient()
        {
            ClearCollection();
            var client = new Client()
            {
                Login = "test_update_login",
                Password = "test_update_password",
                Name = "test_update_name"
            };
            _clientRepository.Add(client);

            // поменяли поля для обновления старого клиента
            client.Login = "updated_login";
            client.Password = "updated_password";
            client.Name = "updated_name";

            // Act
            _clientRepository.Update(client);

            // Assert
            var updatedClient = _clientRepository.Get(client.Id);
            Assert.NotNull(updatedClient);

            Assert.Equal(updatedClient.Login, client.Login);
            Assert.Equal(updatedClient.Password, client.Password);
            Assert.Equal(updatedClient.Name, client.Name);
            ClearCollection();
        }

        [Fact]
        public void GetClientByLogin_ReturnsClientOrNULL()
        {
            ClearCollection();
            var client = new Client()
            {
                Login = "test_getBylogin_login",
                Password = "test_getBylogin_pass",
                Name = "test_getBylogin_name"
            };

            // нет клиента с таким логином
            // Act 1
            var receivedClient = _clientRepository.GetByLogin(client.Login);

            // Assert
            Assert.Null(receivedClient);

            // есть клиент с таким логином
            _clientRepository.Add(client);

            // Act 2
            receivedClient = _clientRepository.GetByLogin(client.Login);

            // Assert
            Assert.NotNull(receivedClient);
            Assert.Equal(receivedClient.Login, client.Login);
            Assert.Equal(receivedClient.Password, client.Password);
            Assert.Equal(receivedClient.Name, client.Name);
            ClearCollection();
        }
    }
}
