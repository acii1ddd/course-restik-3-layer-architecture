using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class OrderItemArchiveRepository : IOrderItemArchiveRepository
    {
        private readonly IMongoCollection<OrderItem> _collection;
        private readonly IMongoCollection<BsonDocument> _counterCollection;

        public OrderItemArchiveRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<OrderItem>(collectionName);
            _counterCollection = database.GetCollection<BsonDocument>("counter");
            // Инициализируем счетчик, если он отсутствует
            InitializeCounter("counter");
        }

        public void Add(OrderItem entity)
        {
            //if (entity.Id == 0) // Если Id не задан
            //{
                //entity.Id = GenerateNextId();
            //}
            //entity.TotalDishPrice = entity.CurrDishPrice * entity.Quantity;
            _collection.InsertOne(entity);
        }

        public void Delete(OrderItem entity)
        {
            var filter = Builders<OrderItem>.Filter.Eq(d => d.Id, entity.Id);
            _collection.DeleteOne(filter);
        }

        public OrderItem? Get(int id)
        {
            var filter = Builders<OrderItem>.Filter.Eq(d => d.Id, id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public IEnumerable<OrderItem> GetAll()
        {
            return _collection.Find(FilterDefinition<OrderItem>.Empty).ToList();
        }

        public void Update(OrderItem entity)
        {
            // пересчитываем totalDishPrice у изменяемого элемента
            //entity.TotalDishPrice = entity.CurrDishPrice * entity.Quantity;
            var filter = Builders<OrderItem>.Filter.Eq(d => d.Id, entity.Id);
            var update = Builders<OrderItem>.Update
                .Set(oi => oi.OrderId, entity.OrderId)
                .Set(oi => oi.DishId, entity.DishId)
                .Set(oi => oi.Quantity, entity.Quantity)
                .Set(oi => oi.CurrDishPrice, entity.CurrDishPrice)
                .Set(oi => oi.TotalDishPrice, entity.TotalDishPrice);

            _collection.UpdateOne(filter, update);
        }

        private int GenerateNextId()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "OrderItemArchiveId");
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
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "OrderItemArchiveId");
            var exists = _counterCollection.Find(filter).FirstOrDefault();
            if (exists == null)
            {
                var document = new BsonDocument
                {
                    { "_id", "OrderItemArchiveId" },
                    { "sequence_value", 0 }
                };
                _counterCollection.InsertOne(document);
            }
        }
    }
}
