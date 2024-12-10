using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Npgsql;

namespace TestDAL.MongoRepositoriesTests
{
    public class RoleRepositoryTests
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMongoCollection<Role> _roleCollection;
        
        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public RoleRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);

            _roleCollection = database.GetCollection<Role>("roles");
            _roleRepository = serviceProvider.GetService<IRoleRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearCollections()
        {
            _roleCollection.DeleteMany(Builders<Role>.Filter.Empty);
        }

        [Fact]
        public void AddRole()
        {
            ClearCollections();
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

            // гетнуть из бд
            var retrievedRole = _roleCollection.Find(r => r.Id == role.Id).FirstOrDefault();

            // Assert
            Assert.Equal(1, role.Id);
            Assert.Equal(role.Name, retrievedRole.Name);

            ClearCollections();
        }

        [Fact]
        public void GetRole()
        {
            ClearCollections();

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
            ClearCollections();
        }

        [Fact]
        public void DeleteRole()
        {
            ClearCollections();
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
            ClearCollections();
        }

        [Fact]
        public void GetAllRoles()
        {
            ClearCollections();
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
            ClearCollections();
        }

        [Fact]
        public void UpdateRole()
        {
            ClearCollections();
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
            var updatedRole = _roleRepository.Get(role.Id);
            Assert.NotNull(updatedRole);
            Assert.Equal(updatedRole.Name, role.Name);
            ClearCollections();
        }
    }
}