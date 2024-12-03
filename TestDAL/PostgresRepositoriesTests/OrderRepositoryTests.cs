using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestDAL.PostgresRepositoriesTests
{
    public class OrderRepositoryTests
    {
        private readonly IOrderRepository _orderRepository;
        private readonly string _testPostgresConnectionString;

        public OrderRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTest(out _testPostgresConnectionString);
            _orderRepository = serviceProvider.GetService<IOrderRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
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

        private void ShureClientExists(int id, string login, string password, string name)
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = $"INSERT INTO clients (id, login, password, name) VALUES (@id, @login, @password, @name) ON CONFLICT DO NOTHING";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@name", name);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void ShureWorkerExists(int id, int roleId, string login, string password, string phoneNumber, DateTime hireDate, string fullName)
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = $"INSERT INTO workers (id, role_id, login, password, phone_number, hire_date, full_name) " +
                    $"VALUES (@id, @role_id, @login, @password, @phone_number, @hire_date, @full_name) ON CONFLICT DO NOTHING";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@role_id", roleId);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@phone_number", phoneNumber);
                    command.Parameters.AddWithValue("@hire_date", hireDate);
                    command.Parameters.AddWithValue("@full_name", fullName);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void ShureRoleExists(int id, string roleName)
        {
            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "INSERT INTO roles (id, name) VALUES (@id, @roleName) ON CONFLICT DO NOTHING";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@roleName", roleName);
                    command.ExecuteNonQuery();
                }
            }
        }

        [Fact]
        public void AddOrder()
        {
            ClearTable("orders");
            // если уже есть роль с id = 1, то ничего не делаем и будем использовать ее, если ее нет - создали
            ShureClientExists(1, "TestClient1", "testPass", "testName");

            // Id = 0
            var order = new Order // локальное время по умолчанию в date
            {
                ClientId = 1,
                TableNumber = 4,
            };

            try
            {
                // Вызов метода добавления клиента, меняется Id в client
                _orderRepository.Add(order);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении клиента: {ex.Message}");
            }

            // Assert
            Assert.NotEqual(0, order.Id);

            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM orders WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", order.Id); // autoinc в бд

                    using (var reader = command.ExecuteReader())
                    {
                        // true если reader вернет true, исключение с сообщением "Order not found in database." - если false
                        Assert.True(reader.Read(), "Order not found in database."); // проверка reader на true/false
                        Assert.Equal(order.ClientId, reader["client_id"]);
                        // сравниваем только дату (хотя в бд - timestamp
                        Assert.Equal(DateTime.UtcNow.Date, ((DateTime)reader["date"]).Date); // в базе данных TimeStamp

                        // индекс столбца total_cost
                        var costIndex = reader.GetOrdinal("total_cost");
                        Assert.True(reader.IsDBNull(costIndex));

                        Assert.Equal(OrderStatus.InProcessing, Enum.Parse<OrderStatus>(reader["status"].ToString()));
                        Assert.Equal(PaymentStatus.Unpaid, Enum.Parse<PaymentStatus>(reader["payment_status"].ToString()));

                        var waiterIndex = reader.GetOrdinal("waiter_id");
                        Assert.True(reader.IsDBNull(waiterIndex));

                        var cookIndex = reader.GetOrdinal("cook_id");
                        Assert.True(reader.IsDBNull(cookIndex));

                        Assert.Equal(order.TableNumber, reader["table_number"]);
                    }
                }
            }
            ClearTable("clients");
            ClearTable("orders");
        }

        [Fact]
        public void GetOrder()
        {
            // очистка табллицы clients
            ClearTable("orders");
            ShureClientExists(1, "TestClient1", "testPass", "testName");

            var order = new Order
            {
                ClientId = 1,
                TableNumber = 4,
            };

            var receivedOrder = new Order();

            // Act
            receivedOrder = _orderRepository.Get(order.Id);
            // Assert
            Assert.Null(receivedOrder); // нет клиента с таким Id - получаем null

            _orderRepository.Add(order);
            // Act
            receivedOrder = _orderRepository.Get(order.Id);
            // Assert
            Assert.NotNull(receivedOrder);
            Assert.Equal(order.ClientId, receivedOrder.ClientId);
            Assert.Equal(DateTime.UtcNow.Date, (receivedOrder.Date).Date); // сравниваем только дату
            Assert.Null(receivedOrder.TotalCost);

            Assert.Equal(OrderStatus.InProcessing, receivedOrder.Status);
            Assert.Equal(PaymentStatus.Unpaid, receivedOrder.PaymentStatus);
            Assert.Null(receivedOrder.WaiterId);
            Assert.Null(receivedOrder.CookId);
            Assert.Equal(order.TableNumber, receivedOrder.TableNumber);

            ClearTable("clients");
            ClearTable("orders");
        }

        [Fact]
        public void DeleteOrder()
        {
            ClearTable("orders");
            ShureClientExists(1, "TestClient1", "testPass", "testName");
            var order = new Order
            {
                ClientId = 1,
                TableNumber = 4,
            };

            _orderRepository.Add(order);

            // Act
            _orderRepository.Delete(order);

            // Assert
            var deletedClient = _orderRepository.Get(order.Id);
            Assert.Null(deletedClient);
            ClearTable("clients");
            ClearTable("orders");
        }

        [Fact]
        public void GetAllOrders()
        {
            ClearTable("orders");
            ShureClientExists(1, "TestClient1", "testPass", "testName");
            var order = new Order
            {
                ClientId = 1,
                TableNumber = 4,
            };

            // пустой лист, если ничего нету в таблице
            var orders = new List<Order>();

            // Act
            orders = _orderRepository.GetAll().ToList();

            // Assert
            Assert.Empty(orders);

            // не пустой лист
            _orderRepository.Add(order);

            // Act
            orders = _orderRepository.GetAll().ToList();

            // Assert
            Assert.Single(orders); // одна штука
            Assert.Contains(orders, o => o.ClientId == order.ClientId);
            Assert.Contains(orders, o => o.TableNumber == order.TableNumber);
            ClearTable("clients");
            ClearTable("orders");
        }

        [Fact]
        public void UpdateOrders()
        {
            ClearTable("orders");
            ShureClientExists(1, "TestClient1", "testPass1", "testName1");
            var order = new Order
            {
                ClientId = 1,
                TableNumber = 4
            };
            _orderRepository.Add(order);

            // поменяли поля для обновления старого сотрудника
            ShureRoleExists(1, "testWaiter");
            ShureRoleExists(2, "testCook");

            // client
            ShureClientExists(2, "TestClient", "testPass", "testName");
            // waiter
            ShureWorkerExists(1, 1, "TestWaiter", "testPass", "+375443455555", new DateTime(), "testName");
            // cook
            ShureWorkerExists(2, 2, "TestCook", "testPass", "+375443455544", new DateTime(), "testName");

            order.ClientId = 2;
            order.Date = DateTime.Parse("2024-11-20");
            order.TotalCost = 20;
            order.Status = OrderStatus.IsCooking;
            order.PaymentStatus = PaymentStatus.Paid;
            order.WaiterId = 1;
            order.CookId = 2;
            order.TableNumber = 5;

            // Act
            _orderRepository.Update(order);

            // Assert
            var updatedOrder = _orderRepository.Get(order.Id);
            Assert.NotNull(updatedOrder);
            Assert.Equal(order.ClientId, updatedOrder.ClientId);
            Assert.Equal(order.Date, (updatedOrder.Date).Date); // сравниваем только дату
            Assert.Equal(order.TotalCost, updatedOrder.TotalCost);
            Assert.Equal(order.Status, updatedOrder.Status);
            Assert.Equal(order.PaymentStatus, updatedOrder.PaymentStatus);
            Assert.Equal(order.WaiterId, updatedOrder.WaiterId);
            Assert.Equal(order.CookId, updatedOrder.CookId);
            Assert.Equal(order.TableNumber, updatedOrder.TableNumber);
            
            ClearTable("roles");
            ClearTable("workers");
            ClearTable("clients");
            ClearTable("orders");
        }
    }
}