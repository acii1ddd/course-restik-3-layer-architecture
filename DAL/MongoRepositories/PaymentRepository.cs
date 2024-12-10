using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<Payment> _collection;

        public PaymentRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Payment>(collectionName);
        }

        public void Add(Payment entity)
        {
            if (entity.Id == 0) // Если Id не задан
            {
                entity.Id = GenerateNextId();
            }
            _collection.InsertOne(entity);
        }

        public void Delete(Payment entity)
        {
            var filter = Builders<Payment>.Filter.Eq(d => d.Id, entity.Id);
            _collection.DeleteOne(filter);
        }

        public Payment? Get(int id)
        {
            var filter = Builders<Payment>.Filter.Eq(d => d.Id, id);
            var payment = _collection.Find(filter).FirstOrDefault();
            if (payment != null)
            {
                // преобразуем дату из UTC в локальный часовой пояс
                payment.PaymentDate = ConvertToLocal(payment.PaymentDate);
            }
            return payment;
        }

        public IEnumerable<Payment> GetAll()
        {
            var payments = _collection.Find(FilterDefinition<Payment>.Empty).ToList();
            foreach (var payment in payments)
            {
                // Преобразуем дату из UTC в локальный часовой пояс
                payment.PaymentDate = ConvertToLocal(payment.PaymentDate);
            }
            return payments;
        }

        public void Update(Payment entity)
        {
            var filter = Builders<Payment>.Filter.Eq(d => d.Id, entity.Id);
            var update = Builders<Payment>.Update
                .Set(p => p.PaymentDate, entity.PaymentDate)
                .Set(p => p.PaymentMethod, entity.PaymentMethod)
                .Set(p => p.OrderId, entity.OrderId);

            _collection.UpdateOne(filter, update);
        }

        private int GenerateNextId()
        {
            var lastPayment = _collection
                .Find(FilterDefinition<Payment>.Empty)
                .SortByDescending(d => d.Id) // самый большой Id (последний)
                .FirstOrDefault();

            return lastPayment == null ? 1 : lastPayment.Id + 1;
        }

        private DateTime ConvertToLocal(DateTime utcDate)
        {
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Minsk"); // Минск
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, localTimeZone);
        }
    }
}
