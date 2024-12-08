using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestDAL.MongoRepositoriesTests
{
    public class IngredientRepositoryTests
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMongoCollection<Ingredient> _collection;
        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public IngredientRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);
            _collection = database.GetCollection<Ingredient>("ingredients");

            _ingredientRepository = serviceProvider.GetService<IIngredientRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        // очистка коллекции перед каждым тестом
        private void ClearCollection()
        {
            _collection.DeleteMany(Builders<Ingredient>.Filter.Empty);
        }

        [Fact]
        public void AddDish()
        {
            ClearCollection();
            // Id = 0
            var ingredient = new Ingredient
            {
                Name = "test_add_name",
                Unit = UnitsOfMeasurement.Kg,
                StockQuantity = 100,
                ThresholdLevel = 1
            };

            try
            {
                // Вызов метода добавления клиента, меняется Id в client
                _ingredientRepository.Add(ingredient);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении клиента: {ex.Message}");
            }

            // получили добавленное блюдо
            var retrievedClient = _collection.Find(c => c.Id == ingredient.Id).FirstOrDefault(); // Поиск клиента асинхронно

            // Assert
            Assert.Equal(1, ingredient.Id); // обновился Id
            Assert.NotNull(retrievedClient);
            Assert.Equal(ingredient.Name, retrievedClient.Name);
            Assert.Equal(ingredient.Unit, retrievedClient.Unit);
            Assert.Equal(ingredient.StockQuantity, retrievedClient.StockQuantity);
            Assert.Equal(ingredient.ThresholdLevel, retrievedClient.ThresholdLevel);

            ClearCollection();
        }

        [Fact]
        public void GetIngredient()
        {
            ClearCollection();

            var ingredient = new Ingredient
            {
                Name = "test_get_name",
                Unit = UnitsOfMeasurement.Kg,
                StockQuantity = 100,
                ThresholdLevel = 1
            };

            var receivedIngredient = new Ingredient();

            // Act
            receivedIngredient = _ingredientRepository.Get(ingredient.Id);
            // Assert
            Assert.Null(receivedIngredient); // нет клиента с таким Id - получаем null

            _ingredientRepository.Add(ingredient);

            // Act
            receivedIngredient = _ingredientRepository.Get(ingredient.Id);
            // Assert
            Assert.NotNull(receivedIngredient);

            Assert.Equal(ingredient.Name, ingredient.Name);
            Assert.Equal(ingredient.Unit, ingredient.Unit);
            Assert.Equal(ingredient.StockQuantity, ingredient.StockQuantity);
            Assert.Equal(ingredient.ThresholdLevel, ingredient.ThresholdLevel);
            ClearCollection();
        }

        [Fact]
        public void DeleteIngredient()
        {
            ClearCollection();
            var ingredient = new Ingredient
            {
                Name = "test_get_name",
                Unit = UnitsOfMeasurement.Kg,
                StockQuantity = 100,
                ThresholdLevel = 1
            };

            _ingredientRepository.Add(ingredient);

            // Act
            _ingredientRepository.Delete(ingredient);

            // Assert
            var deletedClient = _ingredientRepository.Get(ingredient.Id);
            Assert.Null(deletedClient);
            ClearCollection();
        }

        [Fact]
        public void GetAllIngredients()
        {
            ClearCollection();
            var ingredient1 = new Ingredient
            {
                Name = "test_get_name1",
                Unit = UnitsOfMeasurement.Kg,
                StockQuantity = 100,
                ThresholdLevel = 1
            };

            var ingredient2 = new Ingredient
            {
                Name = "test_get_name2",
                Unit = UnitsOfMeasurement.Milliliter,
                StockQuantity = 25,
                ThresholdLevel = 3
            };

            // пустой лист, если ничего нету в таблице
            var ingredients = new List<Ingredient>();

            // Act
            ingredients = _ingredientRepository.GetAll().ToList();

            // Assert
            Assert.Empty(ingredients);

            // не пустой лист
            _ingredientRepository.Add(ingredient1);
            _ingredientRepository.Add(ingredient2);

            // Act
            ingredients = _ingredientRepository.GetAll().ToList();

            // Assert
            Assert.Equal(2, ingredients.Count);

            Assert.Contains(ingredients, i => i.Name == ingredient1.Name);
            Assert.Contains(ingredients, i => i.Name == ingredient2.Name);

            Assert.Contains(ingredients, i => i.Unit == ingredient1.Unit);
            Assert.Contains(ingredients, i => i.Unit == ingredient2.Unit);
            ClearCollection();
        }

        [Fact]
        public void UpdateIngredient()
        {
            ClearCollection();
            var ingredient = new Ingredient
            {
                Name = "test_get_name",
                Unit = UnitsOfMeasurement.Kg,
                StockQuantity = 100,
                ThresholdLevel = 1
            };
            _ingredientRepository.Add(ingredient);

            // поменяли поля для обновления старого клиента
            ingredient.Name = "updated_name";
            ingredient.Unit = UnitsOfMeasurement.Liter;
            ingredient.StockQuantity = 50;
            ingredient.ThresholdLevel = 0.5m; // cуффикс m для decimal

            // Act
            _ingredientRepository.Update(ingredient);

            // Assert
            var updatedIngredient = _ingredientRepository.Get(ingredient.Id);
            Assert.NotNull(updatedIngredient);

            Assert.Equal(updatedIngredient.Name, ingredient.Name);
            Assert.Equal(updatedIngredient.Unit, ingredient.Unit);
            Assert.Equal(updatedIngredient.StockQuantity, ingredient.StockQuantity);
            Assert.Equal(updatedIngredient.ThresholdLevel, ingredient.ThresholdLevel);
            ClearCollection();
        }
    }
}