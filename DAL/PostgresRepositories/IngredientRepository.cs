using DAL.Entities;
using DAL.Interfaces;
using Npgsql;

namespace DAL.PostgresRepositories
{
    internal class IngredientRepository : IIngredientRepository
    {
        private readonly string _connectionString;

        public IngredientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Ingredient entity)
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO ingredients (name, unit, stock_quantity, threshold_level) " +
                    "values (@name, @unit, @stock_quantity, @threshold_level) RETURNING id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@unit", entity.Unit.ToString());
                    command.Parameters.AddWithValue("@stock_quantity", entity.StockQuantity);
                    command.Parameters.AddWithValue("@threshold_level", entity.ThresholdLevel);

                    var id = Convert.ToInt32(command.ExecuteScalar()); // так как returnings id
                    entity.Id = id; // id в бд соответствует id объекта
                }
            }
        }

        public void Delete(Ingredient entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM ingredients WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.ExecuteNonQuery(); // выполн удаление
                }
            }
        }

        public Ingredient? Get(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT id, name, unit, stock_quantity, threshold_level FROM ingredients WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Ingredient
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Unit = Enum.Parse<UnitsOfMeasurement>(reader.GetString(2)),
                                StockQuantity = reader.GetDecimal(3),
                                ThresholdLevel = reader.GetDecimal(4)
                            };
                        }
                    }
                }
            }
            return null; // если не найден
        }

        public IEnumerable<Ingredient> GetAll()
        {
            var ingredients = new List<Ingredient>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM ingredients";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingredients.Add(new Ingredient
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Unit = Enum.Parse<UnitsOfMeasurement>(reader.GetString(2)),
                                StockQuantity = reader.GetDecimal(3),
                                ThresholdLevel = reader.GetDecimal(4)
                            });
                        }
                    }
                }
                return ingredients; // пустой список new List<Client>(), либо список клиентов
            }
        }

        public void Update(Ingredient entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE ingredients SET " +
                            "name = @name, " +
                            "unit = @unit, " +
                            "stock_quantity = @stock_quantity, " +
                            "threshold_level = @threshold_level " +
                            "WHERE id = @id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@unit", entity.Unit.ToString());
                    command.Parameters.AddWithValue("@stock_quantity", entity.StockQuantity);
                    command.Parameters.AddWithValue("@threshold_level", entity.ThresholdLevel);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
