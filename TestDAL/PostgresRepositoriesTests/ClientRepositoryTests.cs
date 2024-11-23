using DAL.Configuration;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestDAL.PostgresRepositoriesTests
{
    public class ClientRepositoryTests
    {
        private readonly IClientRepository _clientRepository;
        private readonly string _testPostgresConnectionString;

        public ClientRepositoryTests()
        {
            // инициализаци€ _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTest(out _testPostgresConnectionString);
            _clientRepository = serviceProvider.GetService<IClientRepository>() ?? throw new InvalidOperationException("—трока подключени€ дл€ TestPostgres не найдена в конфигурации.");
        }

        /// <summary>
        /// очистка таблицы clients перед каждым тестом
        /// </summary>
        private void ClearTable()
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "TRUNCATE TABLE clients RESTART IDENTITY CASCADE";
                
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        [Fact]
        public void AddClient_ShouldAddClient() // пока без async
        {
            ClearTable();
            // Id = 0
            var client = new Client
            {
                Login = "test_add_login",
                Password = "test_add_password",
                Name = "test_add_name",
            };

            try
            {
                // ¬ызов метода добавлени€ клиента, мен€етс€ Id в client
                _clientRepository.Add(client);
            }
            catch (Exception ex)
            {
                Assert.Fail($"ќшибка при добавлении клиента: {ex.Message}");
            }

            // Assert
            Assert.NotEqual(0, client.Id);

            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM clients WHERE id = @id";
                
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", client.Id);

                    using (var reader = command.ExecuteReader())
                    {
                        // true если reader вернет true, исключение с сообщением "Client not found in database." - если false
                        Assert.True(reader.Read(), "Client not found in database."); // проверка reader на true/false
                        Assert.Equal(client.Login, reader["login"]);
                        Assert.Equal(client.Password, reader["password"]);
                        Assert.Equal(client.Name, reader["name"]);
                    }
                }
            }
        }

        [Fact]
        public void GetClient_ShouldReturnClientById()
        {
            // очистка табллицы clients
            ClearTable();

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
        }

        [Fact]
        public void DeleteClient_ShouldDeleteClient()
        {
            ClearTable();
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
        }

        [Fact]
        public void GetAllClients_ShouldReturnAllClients()
        {
            ClearTable();
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
        }

        [Fact]
        public void UpdateClients_ShouldUpdateExistingClient()
        {
            ClearTable();
            var client = new Client()
            {
                Login = "test_update_login",
                Password = "test_update_password",
                Name = "test_update_name"
            };
            _clientRepository.Add(client);

            // помен€ли пол€ дл€ обновлени€ старого клиента
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
        }
    }
}