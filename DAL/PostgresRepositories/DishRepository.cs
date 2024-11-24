using DAL.Entities;
using DAL.Interfaces;
using Npgsql;

namespace DAL.PostgresRepositories
{
    internal class DishRepository : IDishRepository
    {
        private readonly string _connectionString;

        public DishRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Dish entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO dishes (name, price) values (@name, @price) RETURNING id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@price", entity.Price);

                    var id = Convert.ToInt32(command.ExecuteScalar()); // так как returnings id
                    entity.Id = id; // id в бд соответствует id объекта
                }
            }
        }

        public void Delete(Dish entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM dishes WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.ExecuteNonQuery(); // выполн удаление
                }
            }
        }

        public Dish? Get(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT id, name, price, is_available FROM dishes WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Dish
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                IsAvailable = reader.GetBoolean(3)
                            };
                        }
                    }
                }
            }
            return null; // если не найден
        }

        public IEnumerable<Dish> GetAll()
        {
            var dishes = new List<Dish>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM dishes";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dishes.Add(new Dish
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                IsAvailable = reader.GetBoolean(3)
                            });
                        }
                    }
                }
                return dishes; // пустой список new List<Client>(), либо список клиентов
            }
        }

        public void Update(Dish entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE dishes SET " +
                            "name = @name, " +
                            "price = @price, " +
                            "is_available = @is_available " +
                            "WHERE id = @id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@price", entity.Price);
                    command.Parameters.AddWithValue("@is_available", entity.IsAvailable);
                    command.Parameters.AddWithValue("@id", entity.Id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
