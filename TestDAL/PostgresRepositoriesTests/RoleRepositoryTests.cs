using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestDAL.PostgresRepositoriesTests
{
    public class RoleRepositoryTests
    {
        private readonly IRoleRepository _roleRepository;
        private readonly string _testPostgresConnectionString;

        public RoleRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTest(out _testPostgresConnectionString);
            _roleRepository = serviceProvider.GetService<IRoleRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearTable()
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

        [Fact]
        public void AddRole_ShouldAddRole()
        {
            ClearTable();
            // Id = 0
            var role = new Role
            {
                Name = "test_add_role",
            };

            try
            {
                // Вызов метода добавления клиента, меняется Id в client
                _roleRepository.Add(role);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении клиента: {ex.Message}");
            }

            // Assert
            Assert.NotEqual(0, role.Id);

            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM roles WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", role.Id); // autoinc в бд

                    using (var reader = command.ExecuteReader())
                    {
                        // true если reader вернет true, исключение с сообщением "Role not found in database." - если false
                        Assert.True(reader.Read(), "Role not found in database."); // проверка reader на true/false
                        Assert.Equal(role.Name, reader["name"]);
                    }
                }
            }
        }

        [Fact]
        public void GetRole_ShouldReturnRole()
        {
            // очистка табллицы clients
            ClearTable();

            var role = new Role()
            {
                Name = "test_get_role"
            };

            var receivedRole = new Role();

            // Act
            receivedRole = _roleRepository.Get(role.Id);
            // Assert
            Assert.Null(receivedRole); // нет клиента с таким Id - получаем null

            _roleRepository.Add(role);
            // Act
            receivedRole = _roleRepository.Get(role.Id);
            // Assert
            Assert.NotNull(receivedRole);
            Assert.Equal(role.Name, receivedRole.Name);
        }

        [Fact]
        public void DeleteRole_ShouldDeleteRole()
        {
            ClearTable();
            var role = new Role()
            {
                Name = "test_delete_role"
            };

            _roleRepository.Add(role);

            // Act
            _roleRepository.Delete(role);

            // Assert
            var deletedClient = _roleRepository.Get(role.Id);
            Assert.Null(deletedClient);
        }

        [Fact]
        public void GetAllRoles_ShouldReturnAllRoles()
        {
            ClearTable();
            var role1 = new Role()
            {
                Name = "test_getAll_role_name1"
            };

            var role2 = new Role()
            {
                Name = "test_getAll_role_name2"
            };

            // пустой лист, если ничего нету в таблице
            var roles = new List<Role>();

            // Act
            roles = _roleRepository.GetAll().ToList();

            // Assert
            Assert.Empty(roles);

            // не пустой лист
            _roleRepository.Add(role1);
            _roleRepository.Add(role2);

            // Act
            roles = _roleRepository.GetAll().ToList();

            // Assert
            Assert.Equal(2, roles.Count);

            Assert.Contains(roles, c => c.Name == role1.Name);
            Assert.Contains(roles, c => c.Name == role2.Name);
        }

        [Fact]
        public void UpdateClients_ShouldUpdateExistingClient()
        {
            ClearTable();
            var role = new Role()
            {
                Name = "test_role_update_name"
            };
            _roleRepository.Add(role);

            // поменяли поля для обновления старого клиента
            role.Name = "updated_name";

            // Act
            _roleRepository.Update(role);

            // Assert
            var updatedClient = _roleRepository.Get(role.Id);
            Assert.NotNull(updatedClient);
            Assert.Equal(updatedClient.Name, role.Name);
        }
    }
}