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
            // ������������� _testPostgresConnectionString ������ ������
            var serviceProvider = Configuration.ConfigureTestPostgres(out _testPostgresConnectionString);
            _clientRepository = serviceProvider.GetService<IClientRepository>() ?? throw new InvalidOperationException("������ ����������� ��� TestPostgres �� ������� � ������������.");
        }

        /// <summary>
        /// ������� ������� clients ����� ������ ������
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
        public void AddClient_ShouldAddClient() // ���� ��� async
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
                // ����� ������ ���������� �������, �������� Id � client
                _clientRepository.Add(client);
            }
            catch (Exception ex)
            {
                Assert.Fail($"������ ��� ���������� �������: {ex.Message}");
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
                        // true ���� reader ������ true, ���������� � ���������� "Client not found in database." - ���� false
                        Assert.True(reader.Read(), "Client not found in database."); // �������� reader �� true/false
                        Assert.Equal(client.Login, reader["login"]);
                        Assert.Equal(client.Password, reader["password"]);
                        Assert.Equal(client.Name, reader["name"]);
                    }
                }
            }
            ClearTable();
        }

        [Fact]
        public void GetClient_ShouldReturnClientById()
        {
            // ������� �������� clients
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
            Assert.Null(receivedClient); // ��� ������� � ����� Id - �������� null

            _clientRepository.Add(client);
            
            // Act
            receivedClient = _clientRepository.Get(client.Id);
            // Assert
            Assert.NotNull(receivedClient);

            Assert.Equal(client.Login, receivedClient.Login);
            Assert.Equal(client.Password, receivedClient.Password);
            Assert.Equal(client.Name, receivedClient.Name);
            ClearTable();
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
            ClearTable();
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

            // ������ ����, ���� ������ ���� � �������
            var clients = new List<Client>();
            
            // Act
            clients = _clientRepository.GetAll().ToList();

            // Assert
            Assert.Empty(clients);

            // �� ������ ����
            _clientRepository.Add(client1);
            _clientRepository.Add(client2);

            // Act
            clients = _clientRepository.GetAll().ToList();

            // Assert
            Assert.Equal(2, clients.Count);

            Assert.Contains(clients, c => c.Login == client1.Login);
            Assert.Contains(clients, c => c.Login == client2.Login);
            ClearTable();
        }

        [Fact]
        public void UpdateClient_ShouldUpdateExistingClient()
        {
            ClearTable();
            var client = new Client()
            {
                Login = "test_update_login",
                Password = "test_update_password",
                Name = "test_update_name"
            };
            _clientRepository.Add(client);

            // �������� ���� ��� ���������� ������� �������
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
            ClearTable();
        }

        [Fact]
        public void GetClientByLogin_ReturnsClientOrNULL()
        {
            ClearTable();
            var client = new Client()
            {
                Login = "test_getBylogin_login",
                Password = "test_getBylogin_pass",
                Name = "test_getBylogin_name"
            };

            // ��� ������� � ����� �������
            // Act 1
            var receivedClient = _clientRepository.GetByLogin(client.Login);

            // Assert
            Assert.Null(receivedClient);

            // ���� ������ � ����� �������
            _clientRepository.Add(client);

            // Act 2
            receivedClient = _clientRepository.GetByLogin(client.Login);

            // Assert
            Assert.NotNull(receivedClient);
            Assert.Equal(receivedClient.Login, client.Login);
            Assert.Equal(receivedClient.Password, client.Password);
            Assert.Equal(receivedClient.Name, client.Name);
            ClearTable();
        }
    }
}