using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestDAL.MongoRepositoriesTests
{
    public class DishRepositoryTests
    {
        private readonly IDishRepository _dishRepository;
        private readonly IMongoCollection<Dish> _collection;
        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public DishRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);
            _collection = database.GetCollection<Dish>("dishes");

            _dishRepository = serviceProvider.GetService<IDishRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        // очистка коллекции перед каждым тестом
        private void ClearCollection()
        {
            _collection.DeleteMany(Builders<Dish>.Filter.Empty);
        }

        [Fact]
        public void AddDish()
        {
            ClearCollection();
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
            
            // получили добавленное блюдо
            var retrievedClient = _collection.Find(c => c.Id == dish.Id).FirstOrDefault(); // Поиск клиента асинхронно

            // Assert
            Assert.Equal(1, dish.Id); // Проверка, что Id не null
            Assert.NotNull(retrievedClient);
            Assert.Equal(dish.Name, retrievedClient.Name);
            Assert.Equal(dish.Price, retrievedClient.Price);
            Assert.True(dish.IsAvailable);

            ClearCollection();
        }

        [Fact]
        public void GetDish()
        {
            ClearCollection();
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
            ClearCollection();
        }

        [Fact]
        public void DeleteDish()
        {
            ClearCollection();
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
            ClearCollection();
        }

        [Fact]
        public void GetAllDishes()
        {
            ClearCollection();
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
            ClearCollection();
        }

        [Fact]
        public void UpdateDishes()
        {
            ClearCollection();
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
            ClearCollection();
        }
    }
}
