using DAL.Entities;
using DAL.Interfaces;
using Npgsql;

namespace DAL.PostgresRepositories
{
    internal class PaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public PaymentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Payment entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO payments (payment_date, payment_method, order_id) " +
                    "values (@payment_date, @payment_method, @order_id) RETURNING id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@payment_date", entity.PaymentDate);
                    command.Parameters.AddWithValue("@payment_method", entity.PaymentMethod.ToString());
                    command.Parameters.AddWithValue("@order_id", entity.OrderId);

                    var id = Convert.ToInt32(command.ExecuteScalar()); // так как returnings id
                    entity.Id = id; // id в бд соответствует id объекта
                }
            }
        }

        public void Delete(Payment entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM payments WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.ExecuteNonQuery(); // выполн удаление
                }
            }
        }

        public Payment? Get(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM payments WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Payment
                            {
                                Id = reader.GetInt32(0),
                                PaymentDate = reader.GetDateTime(1),
                                PaymentMethod = Enum.Parse<PaymentMethod>(reader.GetString(2)),
                                OrderId = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }
            return null; // если не найден
        }

        public IEnumerable<Payment> GetAll()
        {
            var payments = new List<Payment>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM payments";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(new Payment
                            {
                                Id = reader.GetInt32(0),
                                PaymentDate = reader.GetDateTime(1),
                                PaymentMethod = Enum.Parse<PaymentMethod>(reader.GetString(2)),
                                OrderId = reader.GetInt32(3)
                            });
                        }
                    }
                }
                return payments; // пустой список new List<Payment>(), либо список клиентов
            }
        }

        public void Update(Payment entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE payments SET " +
                            "payment_date = @payment_date, " +
                            "payment_method = @payment_method, " +
                            "order_id = @order_id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@payment_date", entity.PaymentDate);
                    command.Parameters.AddWithValue("@payment_method", entity.PaymentMethod.ToString());
                    command.Parameters.AddWithValue("@order_id", entity.OrderId);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
