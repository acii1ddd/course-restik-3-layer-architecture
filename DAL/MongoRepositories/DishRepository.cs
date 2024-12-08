using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class DishRepository : IDishRepository
    {
        private readonly IMongoCollection<Dish> _collection;

        public DishRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Dish>(collectionName);
        }

        public void Add(Dish entity)
        {
            if (entity.Id == 0) // Если Id не задан
            {
                entity.Id = GenerateNextId();
            }
            _collection.InsertOne(entity);
        }

        public void Delete(Dish entity)
        {
            var filter = Builders<Dish>.Filter.Eq(d => d.Id, entity.Id);
            _collection.DeleteOne(filter);
        }

        public Dish? Get(int id)
        {
            var filter = Builders<Dish>.Filter.Eq(d => d.Id, id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public IEnumerable<Dish> GetAll()
        {
            return _collection.Find(FilterDefinition<Dish>.Empty).ToList();
        }

        public void Update(Dish entity)
        {
            var filter = Builders<Dish>.Filter.Eq(d => d.Id, entity.Id);
            var update = Builders<Dish>.Update
                .Set(d => d.Name, entity.Name)
                .Set(d => d.Price, entity.Price)
                .Set(d => d.IsAvailable, entity.IsAvailable);

            _collection.UpdateOne(filter, update);
        }

        private int GenerateNextId()
        {
            var lastDish = _collection
                .Find(FilterDefinition<Dish>.Empty)
                .SortByDescending(d => d.Id) // самый большой Id (последний)
                .FirstOrDefault();

            return lastDish == null ? 1 : lastDish.Id + 1;
        }
    }
}
