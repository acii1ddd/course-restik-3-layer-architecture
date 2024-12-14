using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestDAL.PostgresRepositoriesTests
{
    public class OrderItemsRepositoryTests
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IOrderRepository _orderRepository;

        private readonly string _testPostgresConnectionString;

        public OrderItemsRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestPostgres(out _testPostgresConnectionString);
            _orderItemRepository = serviceProvider.GetService<IOrderItemRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _dishRepository = serviceProvider.GetService<IDishRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _clientRepository = serviceProvider.GetService<IClientRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
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
            _orderRepository.Add(order);

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                DishId = dish.Id,
                Quantity = 2,
                CurrDishPrice = 50
            };
            return orderItem;
        }

        [Fact(DisplayName = "Добавление позиции заказа: должен вызвать метод Add()")]
        public void AddOrderItem()
        {
            ClearAllTables();
            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            try
            {
                _orderItemRepository.Add(orderItem);
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
                var query = "SELECT * FROM orders_items WHERE id = @id";

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

        [Fact(DisplayName = "Получение позиции заказа по Id: должен вернуть позицию заказа либо null при ее отсутствии")]
        public void GetOrderItem()
        {
            ClearAllTables();

            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            var receivedOrderItem = new OrderItem();

            // Act
            receivedOrderItem = _orderItemRepository.Get(orderItem.Id);
            // Assert
            Assert.Null(receivedOrderItem); // нет клиента с таким Id - получаем null

            _orderItemRepository.Add(orderItem);
            // Act
            receivedOrderItem = _orderItemRepository.Get(orderItem.Id);
            // Assert
            Assert.NotNull(receivedOrderItem);
            Assert.Equal(orderItem.OrderId, receivedOrderItem.OrderId);
            Assert.Equal(orderItem.DishId, receivedOrderItem.DishId);
            Assert.Equal(orderItem.Quantity, receivedOrderItem.Quantity);
            Assert.Equal(orderItem.CurrDishPrice, receivedOrderItem.CurrDishPrice);
            Assert.Equal(orderItem.TotalDishPrice, receivedOrderItem.TotalDishPrice);
            ClearAllTables();
        }

        [Fact(DisplayName = "Удаление позиции заказа: должен вызвать метод Delete()")]
        public void DeleteOrderItem()
        {
            // переписать, сделать запись в orders_archive
            ClearAllTables();

            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            _orderItemRepository.Add(orderItem);

            // Act
            _orderItemRepository.Delete(orderItem);

            // Assert
            var deletedClient = _orderItemRepository.Get(orderItem.Id);
            Assert.Null(deletedClient);
            ClearAllTables();
        }

        [Fact(DisplayName = "Получение всех позиций заказов: должен вернуть список позиций заказов либо пустой список")]
        public void GetAllOrderItems()
        {
            ClearAllTables();
            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            // пустой лист, если ничего нету в таблице
            var orderItems = new List<OrderItem>();

            // Act
            orderItems = _orderItemRepository.GetAll().ToList();

            // Assert
            Assert.Empty(orderItems);

            // сохраним общую сумму до изменения ее в Add
            var totalPrice = orderItem.Quantity * orderItem.CurrDishPrice;

            // не пустой лист
            _orderItemRepository.Add(orderItem);

            // Act
            orderItems = _orderItemRepository.GetAll().ToList();

            // Assert
            Assert.Single(orderItems); // одна штука
            Assert.Contains(orderItems, oi => oi.OrderId == orderItem.OrderId);
            Assert.Contains(orderItems, oi => oi.DishId == orderItem.DishId);
            Assert.Contains(orderItems, oi => oi.Quantity == orderItem.Quantity);
            Assert.Contains(orderItems, oi => oi.CurrDishPrice == orderItem.CurrDishPrice);
            Assert.Contains(orderItems, oi => oi.TotalDishPrice == totalPrice);
            ClearAllTables();
        }

        [Fact(DisplayName = "Обновление позиции заказа: должен вызвать метод Update()")]
        public void UpdateOrderItem()
        {
            ClearAllTables();
            OrderItem orderItem1 = CreateTestOrderItemWithOrderIdAndDishId(1, 1);
            _orderItemRepository.Add(orderItem1);

            // поменяли поля для обновления старого сотрудника
            OrderItem updated = CreateTestOrderItemWithOrderIdAndDishId(2, 2);
            updated.Id = orderItem1.Id;
            updated.Quantity = 5;
            updated.CurrDishPrice = 5;

            var updatedTotalPrice = updated.Quantity * updated.CurrDishPrice;

            // Act
            _orderItemRepository.Update(updated);

            // Assert
            var updatedOrderItem = _orderItemRepository.Get(updated.Id);
            Assert.NotNull(updatedOrderItem);
            Assert.Equal(updatedOrderItem.OrderId, updated.OrderId);
            Assert.Equal(updatedOrderItem.DishId, updated.DishId);
            Assert.Equal(updatedOrderItem.Quantity, updated.Quantity);
            Assert.Equal(updatedOrderItem.CurrDishPrice, updated.CurrDishPrice);
            Assert.Equal(updatedOrderItem.TotalDishPrice, updatedTotalPrice);
            ClearAllTables();
        }
    }
}