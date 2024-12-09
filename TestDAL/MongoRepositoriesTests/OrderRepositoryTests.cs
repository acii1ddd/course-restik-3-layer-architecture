using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestDAL.MongoRepositoriesTests
{
    public class OrderRepositoryTests
    {
        private readonly IOrderRepository _orderRepository;

        private readonly IMongoCollection<Order> _orderCollection;
        private readonly IMongoCollection<Client> _clientCollection;
        private readonly IMongoCollection<Worker> _workerCollection;
        private readonly IMongoCollection<Role> _roleCollection;

        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public OrderRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);

            _orderCollection = database.GetCollection<Order>("orders");
            _clientCollection = database.GetCollection<Client>("clients");
            _workerCollection = database.GetCollection<Worker>("workers");
            _roleCollection = database.GetCollection<Role>("roles");

            _orderRepository = serviceProvider.GetService<IOrderRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearCollections()
        {
            _orderCollection.DeleteMany(Builders<Order>.Filter.Empty);
            _clientCollection.DeleteMany(Builders<Client>.Filter.Empty);
            _workerCollection.DeleteMany(Builders<Worker>.Filter.Empty);
            _roleCollection.DeleteMany(Builders<Role>.Filter.Empty);
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

        private void EnsureWorkerExists(int id, int roleId, string login, string password, string phoneNumber, DateTime hireDate, string fullName)
        {
            var filter = Builders<Worker>.Filter.Eq("id", id);
            var update = Builders<Worker>.Update
                .SetOnInsert("id", id)
                .SetOnInsert("role_id", roleId)
                .SetOnInsert("login", login)
                .SetOnInsert("password", password)
                .SetOnInsert("phone_number", phoneNumber)
                .SetOnInsert("hire_date", hireDate)
                .SetOnInsert("full_name", fullName);

            _workerCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }

        private void EnsureRoleExists(int id, string roleName)
        {
            var filter = Builders<Role>.Filter.Eq("id", id);
            var update = Builders<Role>.Update
                .SetOnInsert("id", id)
                .SetOnInsert("name", roleName);

            _roleCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }

        [Fact]
        public void AddOrder()
        {
            ClearCollections();
            // если уже есть роль с id = 1, то ничего не делаем и будем использовать ее, если ее нет - создали
            EnsureClientExists(1, "TestClient1", "testPass", "testName");

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

            var retrievedOrder = _orderCollection.Find(o => o.Id == order.Id).FirstOrDefault();

            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal(1, order.Id);
            Assert.Equal(order.ClientId, retrievedOrder.ClientId);

            // в бд в utc дата
            Assert.Equal(order.Date.Date, (retrievedOrder.Date).Date); // сравниваем только дату
            Assert.Null(retrievedOrder.TotalCost);

            Assert.Equal(OrderStatus.InProcessing, retrievedOrder.Status);
            Assert.Equal(PaymentStatus.Unpaid, retrievedOrder.PaymentStatus);
            Assert.Null(retrievedOrder.WaiterId);
            Assert.Null(retrievedOrder.CookId);
            Assert.Equal(order.TableNumber, retrievedOrder.TableNumber);
            ClearCollections();
        }

        [Fact]
        public void GetOrder()
        {
            // очистка табллицы clients
            ClearCollections();
            EnsureClientExists(1, "TestClient1", "testPass", "testName");

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
            Assert.Equal(order.Date.Date, (receivedOrder.Date).Date); // сравниваем только дату
            Assert.Null(receivedOrder.TotalCost);

            Assert.Equal(OrderStatus.InProcessing, receivedOrder.Status);
            Assert.Equal(PaymentStatus.Unpaid, receivedOrder.PaymentStatus);
            Assert.Null(receivedOrder.WaiterId);
            Assert.Null(receivedOrder.CookId);
            Assert.Equal(order.TableNumber, receivedOrder.TableNumber);

            ClearCollections();
        }

        [Fact]
        public void DeleteOrder()
        {
            ClearCollections();
            EnsureClientExists(1, "TestClient1", "testPass", "testName");
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
            ClearCollections();
        }

        [Fact]
        public void GetAllOrders()
        {
            ClearCollections();
            EnsureClientExists(1, "TestClient1", "testPass", "testName");
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
            ClearCollections();
        }

        [Fact]
        public void UpdateOrders()
        {
            ClearCollections();
            EnsureClientExists(1, "TestClient1", "testPass", "testName");
            var order = new Order
            {
                ClientId = 1,
                TableNumber = 4
            };
            _orderRepository.Add(order);

            // поменяли поля для обновления старого сотрудника
            EnsureRoleExists(1, "testWaiter");
            EnsureRoleExists(2, "testCook");

            // client
            EnsureClientExists(2, "TestClient", "testPass", "testName");
            // waiter
            EnsureWorkerExists(1, 1, "TestWaiter", "testPass", "+375443455555", new DateTime(), "testName");
            // cook
            EnsureWorkerExists(2, 2, "TestCook", "testPass", "+375443455544", new DateTime(), "testName");

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

            ClearCollections();
        }
    }
}
