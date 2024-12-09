using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestDAL.MongoRepositoriesTests
{
    public class OrderItemRepositoryTests
    {
        private readonly IOrderItemRepository _orderItemRepository;

        private readonly IMongoCollection<OrderItem> _orderItemCollection;
        private readonly IMongoCollection<Dish> _dishCollection;
        private readonly IMongoCollection<Client> _clientCollection;
        private readonly IMongoCollection<Order> _orderCollection;

        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public OrderItemRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);

            _orderItemCollection = database.GetCollection<OrderItem>("orders_items");
            _dishCollection = database.GetCollection<Dish>("dishes");
            _clientCollection = database.GetCollection<Client>("clients");
            _orderCollection = database.GetCollection<Order>("orders");

            _orderItemRepository = serviceProvider.GetService<IOrderItemRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearCollections()
        {
            _orderItemCollection.DeleteMany(Builders<OrderItem>.Filter.Empty);
            _dishCollection.DeleteMany(Builders<Dish>.Filter.Empty);
            _clientCollection.DeleteMany(Builders<Client>.Filter.Empty);
            _orderCollection.DeleteMany(Builders<Order>.Filter.Empty);
        }

        private void EnsureDishExists(int id, string name, decimal price)
        {
            var filter = Builders<Dish>.Filter.Eq(d => d.Id, id);
            var update = Builders<Dish>.Update
                .SetOnInsert(d => d.Id, id)
                .SetOnInsert(d => d.Name, name)
                .SetOnInsert(d => d.Price, price);

            _dishCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }

        private void EnsureClientExists(int id, string login, string password, string name)
        {
            var filter = Builders<Client>.Filter.Eq(c => c.Id, id);
            var update = Builders<Client>.Update
                .SetOnInsert(c => c.Id, id)
                .SetOnInsert(c => c.Login, login)
                .SetOnInsert(c => c.Password, password)
                .SetOnInsert(c => c.Name, name);

            _clientCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }

        private void EnsureOrderExists(int id, int clientId, int tableNumber)
        {
            var order = new Order
            {
                Id = 1,
                ClientId = clientId,
                TableNumber = tableNumber
            };
            _orderCollection.InsertOne(order);
        }

        private OrderItem CreateTestOrderItemWithOrderIdAndDishId(int orderId, int dishId)
        {
            EnsureDishExists(dishId, "test_add_name1", 50);
            EnsureClientExists(1, "test_login", "test_password", "test_name");
            EnsureOrderExists(orderId, 1, 4);

            var orderItem = new OrderItem
            {
                OrderId = orderId,
                DishId = dishId,
                Quantity = 2,
                CurrDishPrice = 50
            };

            //_orderItemCollection.InsertOne(orderItem);
            return orderItem;
        }

        [Fact]
        public void AddOrderItem()
        {
            ClearCollections();
            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);
            // пересчет total_cost при добавлении order_item
            var calcTotalCost = orderItem.Quantity * orderItem.CurrDishPrice;

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

            var retrievedOrder = _orderItemCollection.Find(o => o.Id == orderItem.Id).FirstOrDefault();
                
            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal(1, orderItem.Id);
            Assert.Equal(orderItem.OrderId, retrievedOrder.OrderId);
            Assert.Equal(orderItem.DishId, retrievedOrder.DishId);
            Assert.Equal(orderItem.Quantity, retrievedOrder.Quantity);
            Assert.Equal(orderItem.CurrDishPrice, retrievedOrder.CurrDishPrice);
            Assert.Equal(orderItem.TotalDishPrice, calcTotalCost);

            ClearCollections();
        }

        [Fact]
        public void GetOrderItem()
        {
            ClearCollections();
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
            ClearCollections();
        }

        [Fact]
        public void DeleteOrderItem()
        {
            ClearCollections();

            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            _orderItemRepository.Add(orderItem);

            // Act
            _orderItemRepository.Delete(orderItem);

            // Assert   
            var deletedClient = _orderItemRepository.Get(orderItem.Id);
            Assert.Null(deletedClient);
            ClearCollections();
        }

        [Fact]
        public void GetAllOrderItemsArchive()
        {
            ClearCollections();

            // пустой лист, если ничего нету в таблице
            var orderItems = new List<OrderItem>();

            // Act
            orderItems = _orderItemRepository.GetAll().ToList();

            // Assert
            Assert.Empty(orderItems);

            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);
            _orderItemRepository.Add(orderItem);
            // Act
            orderItems = _orderItemRepository.GetAll().ToList();

            // Assert
            Assert.Single(orderItems); // одна штука
            Assert.Contains(orderItems, oi => oi.OrderId == orderItem.OrderId);
            Assert.Contains(orderItems, oi => oi.DishId == orderItem.DishId);
            Assert.Contains(orderItems, oi => oi.Quantity == orderItem.Quantity);
            Assert.Contains(orderItems, oi => oi.CurrDishPrice == orderItem.CurrDishPrice);
            Assert.Contains(orderItems, oi => oi.TotalDishPrice == orderItem.TotalDishPrice);
            ClearCollections();
        }

        [Fact]
        public void UpdateOrderItemArchive()
        {
            ClearCollections();
            OrderItem orderItem1 = CreateTestOrderItemWithOrderIdAndDishId(1, 1);
            _orderItemRepository.Add(orderItem1);

            // поменяли поля для обновления старого сотрудника
            orderItem1.Quantity = 5;
            orderItem1.CurrDishPrice = 5;
            var updatedTotalPrice = orderItem1.Quantity * orderItem1.CurrDishPrice;
            orderItem1.TotalDishPrice = updatedTotalPrice;

            // Act
            _orderItemRepository.Update(orderItem1);

            // Assert
            var updatedOrderItem = _orderItemRepository.Get(orderItem1.Id);
            Assert.NotNull(updatedOrderItem);
            Assert.Equal(updatedOrderItem.OrderId, orderItem1.OrderId);
            Assert.Equal(updatedOrderItem.DishId, orderItem1.DishId);
            Assert.Equal(updatedOrderItem.Quantity, orderItem1.Quantity);
            Assert.Equal(updatedOrderItem.CurrDishPrice, orderItem1.CurrDishPrice);
            Assert.Equal(updatedOrderItem.TotalDishPrice, updatedTotalPrice);
            ClearCollections();
        }
    }
}