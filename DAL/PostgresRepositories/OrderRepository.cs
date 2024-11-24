using DAL.Entities;
using DAL.Interfaces;
using Npgsql;

namespace DAL.PostgresRepositories
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Order entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO orders (client_id, table_number) " +
                    "values (@client_id, @table_number) RETURNING id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@client_id", entity.ClientId);
                    command.Parameters.AddWithValue("@table_number", entity.TableNumber);

                    var id = Convert.ToInt32(command.ExecuteScalar()); // так как returnings id
                    entity.Id = id; // id в бд соответствует id объекта
                }
            }
        }

        public void Delete(Order entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM orders WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.ExecuteNonQuery(); // выполн удаление
                }
            }
        }

        public Order? Get(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM orders WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Order
                            {
                                Id = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                Date = reader.GetDateTime(2),
                                TotalCost = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                                Status = Enum.Parse<OrderStatus>(reader.GetString(4)), // enum status
                                PaymentStatus = Enum.Parse<PaymentStatus>(reader.GetString(5)),
                                WaiterId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                                CookId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                                TableNumber = reader.GetInt32(8)
                            };
                        }
                    }
                }
            }
            return null; // если не найден
        }

        public IEnumerable<Order> GetAll()
        {
            var orders = new List<Order>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM orders";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                Id = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                Date = reader.GetDateTime(2),
                                TotalCost = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                                Status = Enum.Parse<OrderStatus>(reader.GetString(4)), // enum status
                                PaymentStatus = Enum.Parse<PaymentStatus>(reader.GetString(5)),
                                WaiterId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                                CookId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                                TableNumber = reader.GetInt32(8)
                            });
                        }
                    }
                }
                return orders; // пустой список new List<Worker>(), либо список клиентов
            }
        }

        public void Update(Order entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE orders SET " +
                            "client_id = @client_id, " +
                            "date = @date, " +
                            "total_cost = @total_cost, " +
                            "status = @status, " +
                            "payment_status = @payment_status, " +
                            "waiter_id = @waiter_id, " +
                            "cook_id = @cook_id, " +
                            "table_number = @table_number " +
                            "WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@client_id", entity.ClientId);
                    command.Parameters.AddWithValue("@date", entity.Date);
                    command.Parameters.AddWithValue("@total_cost", entity.TotalCost);
                    command.Parameters.AddWithValue("@status", entity.Status.ToString());
                    command.Parameters.AddWithValue("@payment_status", entity.PaymentStatus.ToString());
                    command.Parameters.AddWithValue("@waiter_id", entity.WaiterId);
                    command.Parameters.AddWithValue("@cook_id", entity.CookId);
                    command.Parameters.AddWithValue("@table_number", entity.TableNumber);
                    command.Parameters.AddWithValue("@id", entity.Id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
