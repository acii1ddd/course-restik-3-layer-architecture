using DAL.Entities;
using DAL.Interfaces;
using Npgsql;

namespace DAL.PostgresRepositories
{
    internal class OrderItemRepository : IOrderItemRepository
    {
        private readonly string _connectionString;

        public OrderItemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(OrderItem entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO orders_items (order_id, dish_id, quantity, curr_dish_price) " +
                    "values (@order_id, @dish_id, @quantity, @curr_dish_price) RETURNING id, total_dish_price";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@order_id", entity.OrderId);
                    command.Parameters.AddWithValue("@dish_id", entity.DishId);
                    command.Parameters.AddWithValue("@quantity", entity.Quantity);
                    command.Parameters.AddWithValue("@curr_dish_price", entity.CurrDishPrice);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.Id = reader.GetInt32(0); // id в бд соответствует id объекта
                            entity.TotalDishPrice = reader.GetDecimal(1);
                        }
                    }
                }
            }
        }

        public void Delete(OrderItem entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM orders_items WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.ExecuteNonQuery(); // выполн удаление
                }
            }
        }

        public OrderItem? Get(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM orders_items WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new OrderItem
                            {
                                Id = reader.GetInt32(0),
                                OrderId = reader.GetInt32(1),
                                DishId = reader.GetInt32(2),
                                Quantity = reader.GetInt32(3),
                                CurrDishPrice = reader.GetDecimal(4),
                                TotalDishPrice = reader.GetDecimal(5)
                            };
                        }
                    }
                }
            }
            return null; // если не найден
        }

        public IEnumerable<OrderItem> GetAll()
        {
            var orderItems = new List<OrderItem>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM orders_items";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderItems.Add(new OrderItem
                            {
                                Id = reader.GetInt32(0),
                                OrderId = reader.GetInt32(1),
                                DishId = reader.GetInt32(2),
                                Quantity = reader.GetInt32(3),
                                CurrDishPrice = reader.GetDecimal(4),
                                TotalDishPrice = reader.GetDecimal(5)
                            });
                        }
                    }
                }
                return orderItems; // пустой список new List<Worker>(), либо список клиентов
            }
        }

        public void Update(OrderItem entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE orders_items SET " +
                            "order_id = @order_id, " +
                            "dish_id = @dish_id, " +
                            "quantity = @quantity, " +
                            "curr_dish_price = @curr_dish_price, " +
                            "total_dish_price = @total_dish_price " +
                            "WHERE id = @id " +
                            "RETURNING total_dish_price"; // Возвращаем новое значение total_dish_count

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@order_id", entity.OrderId);
                    command.Parameters.AddWithValue("@dish_id", entity.DishId);
                    command.Parameters.AddWithValue("@quantity", entity.Quantity);
                    command.Parameters.AddWithValue("@curr_dish_price", entity.CurrDishPrice);
                    command.Parameters.AddWithValue("@total_dish_price", entity.TotalDishPrice);
                    command.Parameters.AddWithValue("@id", entity.Id);

                    var totalDishPrice = command.ExecuteScalar(); // пересчитанный total_dish_price
                    entity.TotalDishPrice = Convert.ToInt32(totalDishPrice);
                }
            }
        }
    }
}
