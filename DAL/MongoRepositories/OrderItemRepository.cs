using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class OrderItemRepository : IOrderItemRepository
    {
        private readonly IMongoCollection<OrderItem> _collection;
        private readonly IMongoDatabase _database;

        public OrderItemRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            _collection = _database.GetCollection<OrderItem>(collectionName);
        }

        public void Add(OrderItem entity)
        {
            if (entity.Id == 0) // Если Id не задан
            {
                entity.Id = GenerateNextId();
            }
            entity.TotalDishPrice = entity.CurrDishPrice * entity.Quantity;
            _collection.InsertOne(entity);

            // Пересчитываем total_cost для заказа
            RecalculateOrderTotalCost(entity.OrderId);
        }

        public void Delete(OrderItem entity)
        {
            var filter = Builders<OrderItem>.Filter.Eq(d => d.Id, entity.Id);
            _collection.DeleteOne(filter);

            // Пересчитываем total_cost для заказа
            RecalculateOrderTotalCost(entity.OrderId);
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
            entity.TotalDishPrice = entity.CurrDishPrice * entity.Quantity;

            var filter = Builders<OrderItem>.Filter.Eq(d => d.Id, entity.Id);
            var update = Builders<OrderItem>.Update
                .Set(oi => oi.OrderId, entity.OrderId)
                .Set(oi => oi.DishId, entity.DishId)
                .Set(oi => oi.Quantity, entity.Quantity)
                .Set(oi => oi.CurrDishPrice, entity.CurrDishPrice)
                .Set(oi => oi.TotalDishPrice, entity.TotalDishPrice);

            _collection.UpdateOne(filter, update);
            
            // Пересчитываем total_cost для заказа
            RecalculateOrderTotalCost(entity.OrderId);
        }

        private int GenerateNextId()
        {
            var lastDish = _collection
                .Find(FilterDefinition<OrderItem>.Empty)
                .SortByDescending(d => d.Id) // самый большой Id (последний)
                .FirstOrDefault();

            return lastDish == null ? 1 : lastDish.Id + 1;
        }

        private void RecalculateOrderTotalCost(int orderId)
        {
            var filter = Builders<OrderItem>.Filter.Eq(oi => oi.OrderId, orderId);
            var orderItems = _collection.Find(filter).ToList();

            decimal totalCost = orderItems.Sum(oi => oi.TotalDishPrice);

            var ordersCollection = _database.GetCollection<Order>("orders");
            var update = Builders<Order>.Update.Set(o => o.TotalCost, totalCost);

            ordersCollection.UpdateOne(o => o.Id == orderId, update);
        }
    }
}
