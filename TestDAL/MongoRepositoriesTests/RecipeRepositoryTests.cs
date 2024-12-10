using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestDAL.MongoRepositoriesTests
{
    public class RecipeRepositoryTests
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IDishRepository _dishRepository;

        private readonly IMongoCollection<Dish> _dishCollection;
        private readonly IMongoCollection<Ingredient> _ingredientCollection;
        private readonly IMongoCollection<Recipe> _recipeCollection;

        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public RecipeRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);

            _dishCollection = database.GetCollection<Dish>("dishes");
            _ingredientCollection = database.GetCollection<Ingredient>("ingredients");
            _recipeCollection = database.GetCollection<Recipe>("recipes");

            _recipeRepository = serviceProvider.GetService<IRecipeRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _dishRepository = serviceProvider.GetService<IDishRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearCollections()
        {
            _dishCollection.DeleteMany(Builders<Dish>.Filter.Empty);
            _ingredientCollection.DeleteMany(Builders<Ingredient>.Filter.Empty);
            _recipeCollection.DeleteMany(Builders<Recipe>.Filter.Empty);
        }

        private Recipe CreateTestRecipeWithDishIdAndIngredientId(int dishId, int ingredientId)
        {
            var dish = new Dish
            {
                Id = dishId,
                Name = "test_add_name",
                Price = 50
            };
            _dishCollection.InsertOne(dish);

            var ingredient = new Ingredient
            {
                Id = ingredientId,
                Name = "test_get_name",
                Unit = UnitsOfMeasurement.Kg,
                StockQuantity = 100,
                ThresholdLevel = 1
            };
            _ingredientCollection.InsertOne(ingredient);

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
            ClearCollections();
            Recipe recipe = CreateTestRecipeWithDishIdAndIngredientId(1, 1);

            try
            {
                _recipeRepository.Add(recipe);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении рецепта: {ex.Message}");
            }

            var retrievedPayment = _recipeCollection.Find(r => r.Id == recipe.Id).FirstOrDefault();

            // Assert
            Assert.Equal(1, recipe.Id);
            Assert.Equal(recipe.DishId, retrievedPayment.DishId);
            Assert.Equal(recipe.IngredientId, retrievedPayment.IngredientId);
            Assert.Equal(recipe.Quantity, retrievedPayment.Quantity);
            Assert.Equal(recipe.Unit, retrievedPayment.Unit);
            
            ClearCollections();
        }

        [Fact]
        public void GetRecipe()
        {
            ClearCollections();

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
            Assert.Equal(1, receivedRecipe.Id);
            Assert.Equal(recipe.DishId, receivedRecipe.DishId);
            Assert.Equal(recipe.IngredientId, receivedRecipe.IngredientId);
            Assert.Equal(recipe.Quantity, receivedRecipe.Quantity);
            Assert.Equal(recipe.Unit, receivedRecipe.Unit);
            ClearCollections();
        }

        [Fact]
        public void DeleteRecipe()
        {
            ClearCollections();

            Recipe recipe = CreateTestRecipeWithDishIdAndIngredientId(1, 1);

            _recipeRepository.Add(recipe);

            // Act
            _recipeRepository.Delete(recipe);

            // Assert
            var deletedClient = _recipeRepository.Get(recipe.Id);
            Assert.Null(deletedClient);
            ClearCollections();
        }

        [Fact]
        public void GetAllRecipes()
        {
            ClearCollections();
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
            Assert.Contains(recipes, re => re.Unit == recipe.Unit);
            ClearCollections();
        }

        [Fact]
        public void UpdateRecipe()
        {
            ClearCollections();
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
            ClearCollections();
        }
    }
}