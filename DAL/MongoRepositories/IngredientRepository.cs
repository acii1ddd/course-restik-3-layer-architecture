using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class IngredientRepository : IIngredientRepository
    {
        private readonly IMongoCollection<Ingredient> _collection;

        public IngredientRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Ingredient>(collectionName);
        }

        public void Add(Ingredient entity)
        {
            if (entity.Id == 0) // Если Id не задан
            {
                entity.Id = GenerateNextId();
            }
            _collection.InsertOne(entity);
        }

        public void Delete(Ingredient entity)
        {
            var filter = Builders<Ingredient>.Filter.Eq(d => d.Id, entity.Id);
            _collection.DeleteOne(filter);
        }

        public Ingredient? Get(int id)
        {
            var filter = Builders<Ingredient>.Filter.Eq(d => d.Id, id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public IEnumerable<Ingredient> GetAll()
        {
            return _collection.Find(FilterDefinition<Ingredient>.Empty).ToList();
        }

        public void Update(Ingredient entity)
        {
            var filter = Builders<Ingredient>.Filter.Eq(d => d.Id, entity.Id);
            var update = Builders<Ingredient>.Update
                .Set(i => i.Name, entity.Name)
                .Set(i => i.Unit, entity.Unit)
                .Set(i => i.StockQuantity, entity.StockQuantity)
                .Set(i => i.ThresholdLevel, entity.ThresholdLevel);

            _collection.UpdateOne(filter, update);
        }

        private int GenerateNextId()
        {
            var lastDish = _collection
                .Find(FilterDefinition<Ingredient>.Empty)
                .SortByDescending(d => d.Id) // самый большой Id (последний)
                .FirstOrDefault();

            return lastDish == null ? 1 : lastDish.Id + 1;
        }
    }
}
