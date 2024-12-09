using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _collection;

        public OrderRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Order>(collectionName);
        }

        public void Add(Order entity)
        {
            if (entity.Id == 0) // Если Id не задан
            {
                entity.Id = GenerateNextId();
            }
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
            var lastDish = _collection
                .Find(FilterDefinition<Order>.Empty)
                .SortByDescending(d => d.Id) // самый большой Id (последний)
                .FirstOrDefault();

            return lastDish == null ? 1 : lastDish.Id + 1;
        }

        private DateTime ConvertToLocal(DateTime utcDate)
        {
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Minsk"); // Минск
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, localTimeZone);
        }
    }
}
