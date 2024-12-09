using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestDAL.MongoRepositoriesTests
{
    public class OrderItemArchiveRepositoryTests
    {
        private readonly IOrderItemArchiveRepository _orderItemArchiveRepository;

        private readonly IMongoCollection<OrderItem> _orderItemArchiveCollection;
        private readonly IMongoCollection<Dish> _dishCollection;
        private readonly IMongoCollection<Client> _clientCollection;
        private readonly IMongoCollection<Order> _orderArchiveCollection;

        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public OrderItemArchiveRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);

            _orderItemArchiveCollection = database.GetCollection<OrderItem>("orders_items_archive");
            _dishCollection = database.GetCollection<Dish>("dishes");
            _clientCollection = database.GetCollection<Client>("clients");
            _orderArchiveCollection = database.GetCollection<Order>("orders_archive");

            _orderItemArchiveRepository = serviceProvider.GetService<IOrderItemArchiveRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearCollections()
        {
            _orderItemArchiveCollection.DeleteMany(Builders<OrderItem>.Filter.Empty);
            _dishCollection.DeleteMany(Builders<Dish>.Filter.Empty);
            _clientCollection.DeleteMany(Builders<Client>.Filter.Empty);
            _orderArchiveCollection.DeleteMany(Builders<Order>.Filter.Empty);
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
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update
                .SetOnInsert(o => o.Id, id)
                .SetOnInsert(o => o.ClientId, clientId)
                .SetOnInsert(o => o.TableNumber, tableNumber);

            _orderArchiveCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
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

            //_orderItemArchiveCollection.InsertOne(orderItem);
            return orderItem;
        }

        [Fact]
        public void AddOrderItemArchive()
        {
            ClearCollections();
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

            var retrievedOrder = _orderItemArchiveCollection.Find(o => o.Id == orderItem.Id).FirstOrDefault();

            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal(1, orderItem.Id);
            Assert.Equal(orderItem.OrderId, retrievedOrder.OrderId);
            Assert.Equal(orderItem.DishId, retrievedOrder.DishId);
            Assert.Equal(orderItem.Quantity, retrievedOrder.Quantity);
            Assert.Equal(orderItem.CurrDishPrice, retrievedOrder.CurrDishPrice);
            Assert.Equal(orderItem.TotalDishPrice, retrievedOrder.TotalDishPrice);
            
            ClearCollections();
        }

        [Fact]
        public void GetOrderItemArchive()
        {
            ClearCollections();
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
            ClearCollections();
        }

        [Fact]
        public void DeleteOrderItemArchive()
        {
            ClearCollections();

            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);

            _orderItemArchiveRepository.Add(orderItem);

            // Act
            _orderItemArchiveRepository.Delete(orderItem);

            // Assert   
            var deletedClient = _orderItemArchiveRepository.Get(orderItem.Id);
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
            orderItems = _orderItemArchiveRepository.GetAll().ToList();

            // Assert
            Assert.Empty(orderItems);

            OrderItem orderItem = CreateTestOrderItemWithOrderIdAndDishId(1, 1);
            _orderItemArchiveRepository.Add(orderItem);
            // Act
            orderItems = _orderItemArchiveRepository.GetAll().ToList();

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
            ClearCollections();
        }
    }
}
