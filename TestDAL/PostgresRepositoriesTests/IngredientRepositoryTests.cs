using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestDAL.PostgresRepositoriesTests
{
    public class IngredientRepositoryTests
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly string _testPostgresConnectionString;

        public IngredientRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTest(out _testPostgresConnectionString);
            _ingredientRepository = serviceProvider.GetService<IIngredientRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearTable()
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "TRUNCATE TABLE ingredients RESTART IDENTITY CASCADE";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        [Fact]
        public void AddIngredientToDb() // пока без async
        {
            ClearTable();
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
                Assert.Fail($"Ошибка при добавлении ингредиента: {ex.Message}");
            }

            // Assert
            Assert.NotEqual(0, ingredient.Id);

            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM ingredients WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", ingredient.Id);

                    using (var reader = command.ExecuteReader())
                    {
                        // true если reader вернет true, исключение с сообщением "Ingredient not found in database." - если false
                        Assert.True(reader.Read(), "Ingredient not found in database."); // проверка reader на true/false
                        Assert.Equal(ingredient.Name, reader["name"]);
                        Assert.Equal(UnitsOfMeasurement.Kg, Enum.Parse<UnitsOfMeasurement>(reader["unit"].ToString()));
                        Assert.Equal(ingredient.StockQuantity, reader["stock_quantity"]);
                        Assert.Equal(ingredient.ThresholdLevel, reader["threshold_level"]);
                    }
                }
            }
            ClearTable();
        }

        [Fact]
        public void GetIngredient()
        {
            // очистка табллицы clients
            ClearTable();

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
            ClearTable();
        }

        [Fact]
        public void DeleteIngredient()
        {
            ClearTable();
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
            ClearTable();
        }

        [Fact]
        public void GetAllIngredients()
        {
            ClearTable();
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
            ClearTable();
        }

        [Fact]
        public void UpdateIngredient()
        {
            ClearTable();
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
            ClearTable();
        }
    }
}
