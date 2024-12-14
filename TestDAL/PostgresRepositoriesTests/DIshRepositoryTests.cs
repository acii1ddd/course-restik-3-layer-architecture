using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestDAL.PostgresRepositoriesTests
{
    public class DIshRepositoryTests
    {
        private readonly IDishRepository _dishRepository;
        private readonly string _testPostgresConnectionString;

        public DIshRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestPostgres(out _testPostgresConnectionString);
            _dishRepository = serviceProvider.GetService<IDishRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearTable()
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "TRUNCATE TABLE dishes RESTART IDENTITY CASCADE";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        [Fact(DisplayName = "Добавление блюда: должен вызвать метод Add()")]
        public void AddDish()
        {
            ClearTable();
            // Id = 0
            var dish = new Dish
            {
                Name = "test_add_name",
                Price = 50
            };

            try
            {
                // Вызов метода добавления клиента, меняется Id в client
                _dishRepository.Add(dish);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении клиента: {ex.Message}");
            }

            // Assert
            Assert.NotEqual(0, dish.Id);

            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM dishes WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", dish.Id);

                    using (var reader = command.ExecuteReader())
                    {
                        // true если reader вернет true, исключение с сообщением "Client not found in database." - если false
                        Assert.True(reader.Read(), "Dish not found in database."); // проверка reader на true/false
                        Assert.Equal(dish.Name, reader["name"]);
                        Assert.Equal(dish.Price, reader["price"]);
                        Assert.True(dish.IsAvailable); // по умолчанию из базы данных - true
                    }
                }
            }
            ClearTable();
        }

        [Fact(DisplayName = "Получение блюда по Id: должен вернуть блюдо либо null при его отсутствии")]
        public void GetDish()
        {
            ClearTable();
            // Id = 0
            var dish = new Dish
            {
                Name = "test_add_name",
                Price = 50
            };

            var receivedDish = new Dish();

            // Act
            receivedDish = _dishRepository.Get(dish.Id);
            // Assert
            Assert.Null(receivedDish); // нет клиента с таким Id - получаем null

            _dishRepository.Add(dish);

            // Act
            receivedDish = _dishRepository.Get(dish.Id);
            // Assert
            Assert.NotNull(receivedDish);

            Assert.Equal(dish.Name, receivedDish.Name);
            Assert.Equal(dish.Price, receivedDish.Price);
            Assert.Equal(dish.IsAvailable, receivedDish.IsAvailable);
            ClearTable();
        }

        [Fact(DisplayName = "Удаление блюда: должен вызвать метод Delete()")]
        public void DeleteDish()
        {
            ClearTable();
            var dish = new Dish
            {
                Name = "test_add_name",
                Price = 50
            };

            _dishRepository.Add(dish);

            // Act
            _dishRepository.Delete(dish);

            // Assert
            var deletedClient = _dishRepository.Get(dish.Id);
            Assert.Null(deletedClient);
            ClearTable();
        }

        [Fact(DisplayName = "Получение всех блюд: должен вернуть список блюд либо пустой список")]
        public void GetAllDishes()
        {
            ClearTable();
            var dish1 = new Dish
            {
                Name = "test_add_name1",
                Price = 50
            };

            var dish2 = new Dish
            {
                Name = "test_add_name2",
                Price = 60
            };

            // пустой лист, если ничего нету в таблице
            var dishes = new List<Dish>();

            // Act
            dishes = _dishRepository.GetAll().ToList();

            // Assert
            Assert.Empty(dishes);

            // не пустой лист
            _dishRepository.Add(dish1);
            _dishRepository.Add(dish2);

            // Act
            dishes = _dishRepository.GetAll().ToList();

            // Assert
            Assert.Equal(2, dishes.Count);

            Assert.Contains(dishes, d => d.Name == dish1.Name);
            Assert.Contains(dishes, d => d.Name == dish2.Name);
            ClearTable();
        }

        [Fact(DisplayName = "Обновление блюда: должен вызвать метод Update()")]
        public void UpdateDishes()
        {
            ClearTable();
            var dish = new Dish
            {
                Name = "test_add_name1",
                Price = 50
            };
            _dishRepository.Add(dish);

            // поменяли поля для обновления старого клиента
            dish.Name = "updated_name";
            dish.Price = 60;
            dish.IsAvailable = false;

            // Act
            _dishRepository.Update(dish);

            // Assert
            var updatedClient = _dishRepository.Get(dish.Id);
            Assert.NotNull(updatedClient);

            Assert.Equal(updatedClient.Name, dish.Name);
            Assert.Equal(updatedClient.Price, dish.Price);
            Assert.Equal(updatedClient.IsAvailable, dish.IsAvailable);
            ClearTable();
        }
    }
}