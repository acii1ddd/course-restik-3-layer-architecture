using DAL.Entities;
using DAL.Interfaces;
using Npgsql;

namespace DAL.PostgresRepositories
{
    internal class RecipeRepository : IRecipeRepository
    {
        private readonly string _connectionString;

        public RecipeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Recipe entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO recipes (dish_id, ingredient_id, quantity, unit) " +
                    "values (@dish_id, @ingredient_id, @quantity, @unit) RETURNING id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dish_id", entity.DishId);
                    command.Parameters.AddWithValue("@ingredient_id", entity.IngredientId);
                    command.Parameters.AddWithValue("@quantity", entity.Quantity);
                    command.Parameters.AddWithValue("@unit", entity.Unit.ToString());

                    var id = Convert.ToInt32(command.ExecuteScalar()); // так как returnings id
                    entity.Id = id; // id в бд соответствует id объекта
                }
            }
        }

        public void Delete(Recipe entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM recipes WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.ExecuteNonQuery(); // выполн удаление
                }
            }
        }

        public Recipe? Get(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT id, dish_id, ingredient_id, quantity, unit FROM recipes WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Recipe
                            {
                                Id = reader.GetInt32(0),
                                DishId = reader.GetInt32(1),
                                IngredientId = reader.GetInt32(2),
                                Quantity = reader.GetDecimal(3),
                                Unit = Enum.Parse<UnitsOfMeasurement>(reader.GetString(4)),
                            };
                        }
                    }
                }
            }
            return null; // если не найден
        }

        public IEnumerable<Recipe> GetAll()
        {
            var recipes = new List<Recipe>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM recipes";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recipes.Add(new Recipe
                            {
                                Id = reader.GetInt32(0),
                                DishId = reader.GetInt32(1),
                                IngredientId = reader.GetInt32(2),
                                Quantity = reader.GetDecimal(3),
                                Unit = Enum.Parse<UnitsOfMeasurement>(reader.GetString(4)),
                            });
                        }
                    }
                }
                return recipes; // пустой список new List<Recipe>(), либо список клиентов
            }
        }

        public void Update(Recipe entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE recipes SET " +
                            "dish_id = @dish_id, " +
                            "ingredient_id = @ingredient_id, " +
                            "quantity = @quantity, " +
                            "unit = @unit " +
                            "WHERE id = @id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dish_id", entity.DishId);
                    command.Parameters.AddWithValue("@ingredient_id", entity.IngredientId);
                    command.Parameters.AddWithValue("@quantity", entity.Quantity);
                    command.Parameters.AddWithValue("@unit", entity.Unit.ToString());
                    command.Parameters.AddWithValue("@id", entity.Id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
