using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestDAL.PostgresRepositoriesTests
{
    public class RecipeRepositoryTests
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly string _testPostgresConnectionString;

        public RecipeRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestPostgres(out _testPostgresConnectionString);
            _recipeRepository = serviceProvider.GetService<IRecipeRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _dishRepository = serviceProvider.GetService<IDishRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _ingredientRepository = serviceProvider.GetService<IIngredientRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearTable(string tableName)
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = $"TRUNCATE TABLE {tableName} RESTART IDENTITY CASCADE";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void ClearAllTables()
        {
            ClearTable("recipes");
            ClearTable("ingredients");
            ClearTable("dishes");
        }

        // dishId - id под которым будет создано блюдо ingredientId - id под которым будет создан ингредиент. Они оба будут прокинуты в recipe
        private Recipe CreateTestRecipeWithDishIdAndIngredientId(int dishId, int ingredientId)
        {
            var dish = new Dish
            {
                Id = dishId,
                Name = "test_add_name",
                Price = 50
            };
            _dishRepository.Add(dish);

            var ingredient = new Ingredient
            {
                Id = ingredientId,
                Name = "test_get_name",
                Unit = UnitsOfMeasurement.Kg,
                StockQuantity = 100,
                ThresholdLevel = 1
            };
            _ingredientRepository.Add(ingredient);

            var recipe = new Recipe
            {
                DishId = dish.Id,
                IngredientId = ingredient.Id,
                Quantity = 2,
                Unit = UnitsOfMeasurement.Gram
            };
            return recipe;
        }

        [Fact]
        public void AddRecipe()
        {
            ClearAllTables();
            Recipe recipe = CreateTestRecipeWithDishIdAndIngredientId(1, 1);

            try
            {
                _recipeRepository.Add(recipe);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении рецепта: {ex.Message}");
            }

            // Assert
            Assert.NotEqual(0, recipe.Id);

            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM recipes WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", recipe.Id); // autoinc в бд

                    using (var reader = command.ExecuteReader())
                    {
                        // true если reader вернет true, исключение с сообщением "Recipe not found in database." - если false
                        Assert.True(reader.Read(), "Recipe not found in database."); // проверка reader на true/false
                        Assert.Equal(recipe.DishId, reader["dish_id"]);
                        Assert.Equal(recipe.IngredientId, reader["ingredient_id"]);
                        Assert.Equal(recipe.Quantity, reader["quantity"]);
                        Assert.Equal(recipe.Unit, Enum.Parse<UnitsOfMeasurement>(reader["unit"].ToString()));
                    }
                }
            }
            ClearAllTables();
        }

        [Fact]
        public void GetRecipe()
        {
            ClearAllTables();

            Recipe recipe = CreateTestRecipeWithDishIdAndIngredientId(1, 1);

            var receivedRecipe = new Recipe();

            // Act
            receivedRecipe = _recipeRepository.Get(recipe.Id);
            // Assert
            Assert.Null(receivedRecipe); // нет клиента с таким Id - получаем null

            _recipeRepository.Add(recipe);
            // Act
            receivedRecipe = _recipeRepository.Get(recipe.Id);
            // Assert
            Assert.NotNull(receivedRecipe);
            Assert.Equal(recipe.DishId, receivedRecipe.DishId);
            Assert.Equal(recipe.IngredientId, receivedRecipe.IngredientId);
            Assert.Equal(recipe.Quantity, receivedRecipe.Quantity);
            Assert.Equal(recipe.Unit, receivedRecipe.Unit);
            ClearAllTables();
        }

        [Fact]
        public void DeleteRecipe()
        {
            ClearAllTables();

            Recipe recipe = CreateTestRecipeWithDishIdAndIngredientId(1, 1);

            _recipeRepository.Add(recipe);

            // Act
            _recipeRepository.Delete(recipe);

            // Assert
            var deletedClient = _recipeRepository.Get(recipe.Id);
            Assert.Null(deletedClient);
            ClearAllTables();
        }

        [Fact]
        public void GetAllRecipes()
        {
            ClearAllTables();
            Recipe recipe = CreateTestRecipeWithDishIdAndIngredientId(1, 1);

            // пустой лист, если ничего нету в таблице
            var recipes = new List<Recipe>();

            // Act
            recipes = _recipeRepository.GetAll().ToList();

            // Assert
            Assert.Empty(recipes);

            // не пустой лист
            _recipeRepository.Add(recipe);

            // Act
            recipes = _recipeRepository.GetAll().ToList();

            // Assert
            Assert.Single(recipes); // одна штука
            Assert.Contains(recipes, re => re.DishId == recipe.DishId);
            Assert.Contains(recipes, re => re.IngredientId == recipe.IngredientId);
            Assert.Contains(recipes, re => re.Quantity == recipe.Quantity);
            Assert.Contains(recipes, re => re.Unit== recipe.Unit);
            ClearAllTables();
        }

        [Fact]
        public void UpdateRecipe()
        {
            ClearAllTables();
            Recipe recipe = CreateTestRecipeWithDishIdAndIngredientId(1, 1);
            _recipeRepository.Add(recipe);

            var dish = new Dish
            {
                Name = "test_add_name",
                Price = 50
            };
            _dishRepository.Add(dish);

            // поменяли поля для обновления старого клиента
            recipe.DishId = dish.Id;
            recipe.Quantity = 250.5m;
            recipe.Unit = UnitsOfMeasurement.Liter;

            // Act
            _recipeRepository.Update(recipe);

            // Assert
            var updatedRecipe = _recipeRepository.Get(recipe.Id);
            Assert.NotNull(updatedRecipe);

            Assert.Equal(updatedRecipe.DishId, recipe.DishId);
            Assert.Equal(updatedRecipe.IngredientId, recipe.IngredientId); // не изменяли - одинаковы с изначальным добавляемым
            Assert.Equal(updatedRecipe.Quantity, recipe.Quantity);
            Assert.Equal(updatedRecipe.Unit, recipe.Unit);
            ClearAllTables();
        }
    }
}
