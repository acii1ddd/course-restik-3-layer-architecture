using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestDAL.PostgresRepositoriesTests
{
    public class PaymentRepositoryTests
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly string _testPostgresConnectionString;

        public PaymentRepositoryTests()
        {
            var serviceProvider = Configuration.ConfigureTestPostgres(out _testPostgresConnectionString);
            _paymentRepository = serviceProvider.GetService<IPaymentRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _orderRepository = serviceProvider.GetService<IOrderRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _clientRepository = serviceProvider.GetService<IClientRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
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
            ClearTable("orders");
            ClearTable("clients");
            ClearTable("payments");
        }

        private Payment CreateTestPaymentWithOrder(int orderId, int dishId)
        {
            var client = new Client
            {
                Id = 1,
                Login = "test_login",
                Password = "test_password",
                Name = "test_name"
            };
            _clientRepository.Add(client);

            var order = new Order
            {
                Id = orderId,
                ClientId = client.Id,
                TableNumber = 4
            };
            _orderRepository.Add(order);

            var payment = new Payment
            {
                PaymentDate = DateTime.Now,
                PaymentMethod = PaymentMethod.Card,
                OrderId = order.Id
            };
            return payment;
        }

        [Fact]
        public void AddPaymentToDb()
        {
            ClearAllTables();
            Payment payment = CreateTestPaymentWithOrder(1, 1);

            try
            {
                // Вызов метода добавления клиента, меняется Id в client
                _paymentRepository.Add(payment);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении клиента: {ex.Message}");
            }

            // Assert
            Assert.NotEqual(0, payment.Id);

            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM payments WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", payment.Id);

                    using (var reader = command.ExecuteReader())
                    {
                        Assert.True(reader.Read(), "Payment not found in database."); // проверка reader на true/false
                        Assert.Equal(payment.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(reader["payment_date"]).ToString("yyyy-MM-dd HH:mm:ss")); // округление даты до секунд
                        Assert.Equal(PaymentMethod.Card, Enum.Parse<PaymentMethod>(reader["payment_method"].ToString()));
                        Assert.Equal(payment.OrderId, reader["order_id"]);
                    }
                }
            }
            ClearAllTables();
        }

        [Fact]
        public void GetPayment()
        {
            ClearAllTables();
            Payment payment = CreateTestPaymentWithOrder(1, 1);

            var receivedPayment = new Payment();

            // Act
            receivedPayment = _paymentRepository.Get(payment.Id);
            // Assert
            Assert.Null(receivedPayment); // нет клиента с таким Id - получаем null

            _paymentRepository.Add(payment);
            // Act
            receivedPayment = _paymentRepository.Get(payment.Id);
            // Assert
            Assert.NotNull(receivedPayment);
            Assert.Equal(payment.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"), receivedPayment.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss")); // округление даты до секунд
            Assert.Equal(payment.PaymentMethod, receivedPayment.PaymentMethod);
            Assert.Equal(payment.OrderId, receivedPayment.OrderId);
            
            ClearAllTables();
        }

        [Fact]
        public void DeletePayment()
        {
            ClearAllTables();
            Payment payment = CreateTestPaymentWithOrder(1, 1);

            _paymentRepository.Add(payment);

            // Act
            _paymentRepository.Delete(payment);

            // Assert
            var deletedPayment = _paymentRepository.Get(payment.Id);
            Assert.Null(deletedPayment);
            
            ClearAllTables();
        }

        [Fact]
        public void GetAllPayments()
        {
            ClearAllTables();
            Payment payment = CreateTestPaymentWithOrder(1, 1);

            // пустой лист, если ничего нету в таблице
            var payments = new List<Payment>();

            // Act
            payments = _paymentRepository.GetAll().ToList();

            // Assert
            Assert.Empty(payments);

            // не пустой лист
            _paymentRepository.Add(payment);

            // Act
            payments = _paymentRepository.GetAll().ToList();

            // Assert
            Assert.Single(payments); // одна штука
            Assert.Contains(payments, p => p.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss") == payment.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.Contains(payments, p => p.PaymentMethod == payment.PaymentMethod);
            Assert.Contains(payments, p => p.OrderId == payment.OrderId);

            ClearAllTables();
        }

        [Fact]
        public void UpdatePayment()
        {
            ClearAllTables();
            Payment payment = CreateTestPaymentWithOrder(1, 1);

            _paymentRepository.Add(payment);

            var order = new Order
            {
                ClientId = 1,
                TableNumber = 4
            };
            _orderRepository.Add(order);

            payment.PaymentDate = DateTime.Parse("2024-11-20");
            payment.PaymentMethod = PaymentMethod.Cash;
            payment.OrderId = order.Id;

            // Act
            _paymentRepository.Update(payment);

            // Assert
            var updatedPayment = _paymentRepository.Get(payment.Id);
            Assert.NotNull(updatedPayment);
            Assert.Equal(payment.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"), updatedPayment.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss")); // сравниваем только дату
            Assert.Equal(payment.PaymentMethod, updatedPayment.PaymentMethod);
            Assert.Equal(payment.OrderId, updatedPayment.OrderId);

            ClearAllTables();
        }
    }
}
