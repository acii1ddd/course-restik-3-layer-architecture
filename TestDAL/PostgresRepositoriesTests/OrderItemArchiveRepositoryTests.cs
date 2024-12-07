using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestDAL.PostgresRepositoriesTests
{
    public class OrderItemArchiveRepositoryTests
    {
        private readonly IOrderItemArchiveRepository _orderItemArchiveRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IOrderArchiveRepository _orderArchiveRepository;

        private readonly string _testPostgresConnectionString;

        public OrderItemArchiveRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTest(out _testPostgresConnectionString);
            _orderItemArchiveRepository = serviceProvider.GetService<IOrderItemArchiveRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _dishRepository = serviceProvider.GetService<IDishRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _clientRepository = serviceProvider.GetService<IClientRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _orderArchiveRepository = serviceProvider.GetService<IOrderArchiveRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
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
            ClearTable("orders_items");
            ClearTable("orders");
            ClearTable("dishes");
            ClearTable("clients");
        }

        // Добавление тестовых данных
        private OrderItem CreateTestOrderItemWithOrderIdAndDishId(int orderId, int dishId)
        {
            var dish = new Dish
            {
                Id = dishId,
                Name = "test_add_name1",
                Price = 50
            };
            _dishRepository.Add(dish);

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
            _orderArchiveRepository.Add(order);

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                DishId = dish.Id,
                Quantity = 2,
                CurrDishPrice = 50
            };
            return orderItem;
        }

        [Fact]
        public void AddOrderItemArchive()
        {
            ClearAllTables();
            // триггера на расчет total_dish_price в этой таблице нет, поэтому поставили его сами
            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            try
            {
                _orderItemArchiveRepository.Add(orderItem);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ошибка при добавлении клиента: {ex.Message}");
            }

            // Assert
            Assert.NotEqual(0, orderItem.Id);

            using (var connection = new NpgsqlConnection(_testPostgresConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM orders_items_archive WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", orderItem.Id); // autoinc в бд

                    using (var reader = command.ExecuteReader())
                    {
                        // true если reader вернет true, исключение с сообщением "OrderItem not found in database." - если false
                        Assert.True(reader.Read(), "OrderItem not found in database."); // проверка reader на true/false
                        Assert.Equal(orderItem.OrderId, reader["order_id"]);
                        Assert.Equal(orderItem.DishId, reader["dish_id"]);
                        Assert.Equal(orderItem.Quantity, reader["quantity"]);
                        Assert.Equal(orderItem.CurrDishPrice, reader["curr_dish_price"]);
                        Assert.Equal(orderItem.TotalDishPrice, reader["total_dish_price"]);
                    }
                }
            }
            ClearAllTables();
        }


        [Fact]
        public void GetOrderItemArchive()
        {
            ClearAllTables();

            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            var receivedOrderItem = new OrderItem();

            // Act
            receivedOrderItem = _orderItemArchiveRepository.Get(orderItem.Id);
            // Assert
            Assert.Null(receivedOrderItem); // нет клиента с таким Id - получаем null

            _orderItemArchiveRepository.Add(orderItem);
            // Act
            receivedOrderItem = _orderItemArchiveRepository.Get(orderItem.Id);
            // Assert
            Assert.NotNull(receivedOrderItem);
            Assert.Equal(orderItem.OrderId, receivedOrderItem.OrderId);
            Assert.Equal(orderItem.DishId, receivedOrderItem.DishId);
            Assert.Equal(orderItem.Quantity, receivedOrderItem.Quantity);
            Assert.Equal(orderItem.CurrDishPrice, receivedOrderItem.CurrDishPrice);
            Assert.Equal(orderItem.TotalDishPrice, receivedOrderItem.TotalDishPrice);
            ClearAllTables();
        }

        [Fact]
        public void DeleteOrderItemArchive()
        {
            ClearAllTables();

            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            _orderItemArchiveRepository.Add(orderItem);

            // Act
            _orderItemArchiveRepository.Delete(orderItem);

            // Assert   
            var deletedClient = _orderItemArchiveRepository.Get(orderItem.Id);
            Assert.Null(deletedClient);
            ClearAllTables();
        }

        [Fact]
        public void GetAllOrderItemsArchive()
        {
            ClearAllTables();
            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            // пустой лист, если ничего нету в таблице
            var orderItems = new List<OrderItem>();

            // Act
            orderItems = _orderItemArchiveRepository.GetAll().ToList();

            // Assert
            Assert.Empty(orderItems);

            // сохраним общую сумму до изменения ее в Add
            var totalPrice = orderItem.Quantity * orderItem.CurrDishPrice; // так как триггера на расчет total_dish_price для orders_items_archive нету

            // не пустой лист
            _orderItemArchiveRepository.Add(orderItem);

            // Act
            orderItems = _orderItemArchiveRepository.GetAll().ToList();

            // Assert
            Assert.Single(orderItems); // одна штука
            Assert.Contains(orderItems, oi => oi.OrderId == orderItem.OrderId);
            Assert.Contains(orderItems, oi => oi.DishId == orderItem.DishId);
            Assert.Contains(orderItems, oi => oi.Quantity == orderItem.Quantity);
            Assert.Contains(orderItems, oi => oi.CurrDishPrice == orderItem.CurrDishPrice);
            Assert.Contains(orderItems, oi => oi.TotalDishPrice == totalPrice);
            ClearAllTables();
        }

        [Fact]
        public void UpdateOrderItemArchive()
        {
            ClearAllTables();
            OrderItem orderItem1 = CreateTestOrderItemWithOrderIdAndDishId(1, 1);
            _orderItemArchiveRepository.Add(orderItem1);

            // поменяли поля для обновления старого сотрудника
            orderItem1.Quantity = 5;
            orderItem1.CurrDishPrice = 5;
            var updatedTotalPrice = orderItem1.Quantity * orderItem1.CurrDishPrice;
            orderItem1.TotalDishPrice = updatedTotalPrice;

            // Act
            _orderItemArchiveRepository.Update(orderItem1);

            // Assert
            var updatedOrderItem = _orderItemArchiveRepository.Get(orderItem1.Id);
            Assert.NotNull(updatedOrderItem);
            Assert.Equal(updatedOrderItem.OrderId, orderItem1.OrderId);
            Assert.Equal(updatedOrderItem.DishId, orderItem1.DishId);
            Assert.Equal(updatedOrderItem.Quantity, orderItem1.Quantity);
            Assert.Equal(updatedOrderItem.CurrDishPrice, orderItem1.CurrDishPrice);
            Assert.Equal(updatedOrderItem.TotalDishPrice, updatedTotalPrice);
            ClearAllTables();
        }
    }
}