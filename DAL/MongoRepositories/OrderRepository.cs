using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _collection;
        private readonly IMongoCollection<BsonDocument> _counterCollection;

        public OrderRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Order>(collectionName);

            _counterCollection = database.GetCollection<BsonDocument>("counter");
            // Инициализируем счетчик, если он отсутствует
            InitializeCounter("counter");
        }

        public void Add(Order entity)
        {
            entity.Id = GenerateNextId(); // Получаем уникальный Id
            _collection.InsertOne(entity);
        }

        public void Delete(Order entity)
        {
            var filter = Builders<Order>.Filter.Eq(d => d.Id, entity.Id);
            _collection.DeleteOne(filter);
        }

        public Order? Get(int id)
        {
            var filter = Builders<Order>.Filter.Eq(d => d.Id, id);
            var order = _collection.Find(filter).FirstOrDefault();
            if (order != null)
            {
                // преобразуем дату из UTC в локальный часовой пояс
                order.Date = ConvertToLocal(order.Date);
            }
            return order;
        }

        public IEnumerable<Order> GetAll()
        {
            var orders = _collection.Find(FilterDefinition<Order>.Empty).ToList();
            foreach (var order in orders)
            {
                // Преобразуем дату из UTC в локальный часовой пояс
                order.Date = ConvertToLocal(order.Date);
            }
            return orders;
        }

        public void Update(Order entity)
        {
            var filter = Builders<Order>.Filter.Eq(d => d.Id, entity.Id);
            var update = Builders<Order>.Update
                .Set(o => o.ClientId, entity.ClientId)
                .Set(o => o.Date, entity.Date)
                .Set(d => d.TotalCost, entity.TotalCost)
                .Set(d => d.Status, entity.Status)
                .Set(d => d.PaymentStatus, entity.PaymentStatus)
                .Set(d => d.WaiterId, entity.WaiterId)
                .Set(d => d.CookId, entity.CookId)
                .Set(d => d.TableNumber, entity.TableNumber);

            _collection.UpdateOne(filter, update);
        }

        private int GenerateNextId()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "OrderId");
            var update = Builders<BsonDocument>.Update.Inc("sequence_value", 1);
            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var result = _counterCollection.FindOneAndUpdate(filter, update, options);
            return result["sequence_value"].AsInt32;
        }

        private void InitializeCounter(string counterCollectionName)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "OrderId");
            var exists = _counterCollection.Find(filter).FirstOrDefault();
            if (exists == null)
            {
                var document = new BsonDocument
                {
                    { "_id", "OrderId" },
                    { "sequence_value", 0 }
                };
                _counterCollection.InsertOne(document);
            }
        }

        private DateTime ConvertToLocal(DateTime utcDate)
        {
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Minsk"); // Минск
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, localTimeZone);
        }
    }
}
