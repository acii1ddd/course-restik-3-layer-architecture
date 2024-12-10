using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestDAL.MongoRepositoriesTests
{
    public class PaymentRepositoryTests
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        private readonly IMongoCollection<Payment> _paymentCollection;
        private readonly IMongoCollection<Order> _orderCollection;
        private readonly IMongoCollection<Client> _clientCollection;

        private readonly string _testMongoConnectionString;
        private readonly string _testMongoDbName;

        public PaymentRepositoryTests()
        {
            // инициализация _testPostgresConnectionString внутри метода
            var serviceProvider = Configuration.ConfigureTestMongo(out _testMongoConnectionString, out _testMongoDbName);

            var client = new MongoClient(_testMongoConnectionString);
            var database = client.GetDatabase(_testMongoDbName);

            _orderCollection = database.GetCollection<Order>("orders");
            _clientCollection = database.GetCollection<Client>("clients");
            _paymentCollection = database.GetCollection<Payment>("payments");

            _paymentRepository = serviceProvider.GetService<IPaymentRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
            _orderRepository = serviceProvider.GetService<IOrderRepository>() ?? throw new InvalidOperationException("Строка подключения для TestPostgres не найдена в конфигурации.");
        }

        private void ClearCollections()
        {
            _orderCollection.DeleteMany(Builders<Order>.Filter.Empty);
            _clientCollection.DeleteMany(Builders<Client>.Filter.Empty);
            _paymentCollection.DeleteMany(Builders<Payment>.Filter.Empty);
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
            _clientCollection.InsertOne(client);

            var order = new Order
            {
                Id = orderId,
                ClientId = client.Id,
                TableNumber = 4
            };
            _orderCollection.InsertOne(order);

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
            ClearCollections();
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

            var retrievedPayment = _paymentCollection.Find(o => o.Id == payment.Id).FirstOrDefault();

            // Assert
            Assert.Equal(1, payment.Id);
            Assert.NotNull(retrievedPayment);
            Assert.Equal(DateTime.UtcNow.Date, retrievedPayment.PaymentDate.Date);
            Assert.Equal(retrievedPayment.PaymentMethod, payment.PaymentMethod);
            Assert.Equal(retrievedPayment.OrderId, payment.OrderId);

            ClearCollections();
        }

        [Fact]
        public void GetPayment()
        {
            ClearCollections();
            Payment payment = CreateTestPaymentWithOrder(1, 1);

            var receivedPayment = new Payment();

            // Act
            receivedPayment = _paymentRepository.Get(payment.Id);
            // Assert
            Assert.Null(receivedPayment);

            _paymentRepository.Add(payment);

            // Act
            receivedPayment = _paymentRepository.Get(payment.Id); // date обновилась
            // Assert
            Assert.NotNull(receivedPayment);
            Assert.Equal(payment.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"), receivedPayment.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss")); // округление даты до секунд
            Assert.Equal(payment.PaymentMethod, receivedPayment.PaymentMethod);
            Assert.Equal(payment.OrderId, receivedPayment.OrderId);

            ClearCollections();
        }

        [Fact]
        public void DeletePayment()
        {
            ClearCollections();
            Payment payment = CreateTestPaymentWithOrder(1, 1);

            _paymentRepository.Add(payment);

            // Act
            _paymentRepository.Delete(payment);

            // Assert
            var deletedPayment = _paymentRepository.Get(payment.Id);
            Assert.Null(deletedPayment);

            ClearCollections();
        }

        [Fact]
        public void GetAllPayments()
        {
            ClearCollections();
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

            ClearCollections();
        }

        [Fact]
        public void UpdatePayment()
        {
            ClearCollections();
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

            ClearCollections();
        }
    }
}