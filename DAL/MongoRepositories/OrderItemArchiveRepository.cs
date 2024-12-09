using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class OrderItemArchiveRepository : IOrderItemArchiveRepository
    {
        private readonly IMongoCollection<OrderItem> _collection;

        public OrderItemArchiveRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<OrderItem>(collectionName);
        }

        public void Add(OrderItem entity)
        {
            if (entity.Id == 0) // Если Id не задан
            {
                entity.Id = GenerateNextId();
            }
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
            var lastDish = _collection
                .Find(FilterDefinition<OrderItem>.Empty)
                .SortByDescending(d => d.Id) // самый большой Id (последний)
                .FirstOrDefault();

            return lastDish == null ? 1 : lastDish.Id + 1;
        }
    }
}
