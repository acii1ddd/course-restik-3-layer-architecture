using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class RecipeRepository : IRecipeRepository
    {
        private readonly IMongoCollection<Recipe> _collection;

        public RecipeRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Recipe>(collectionName);
        }

        public void Add(Recipe entity)
        {
            if (entity.Id == 0) // Если Id не задан
            {
                entity.Id = GenerateNextId();
            }
            _collection.InsertOne(entity);
        }

        public void Delete(Recipe entity)
        {
            var filter = Builders<Recipe>.Filter.Eq(d => d.Id, entity.Id);
            _collection.DeleteOne(filter);
        }

        public Recipe? Get(int id)
        {
            var filter = Builders<Recipe>.Filter.Eq(d => d.Id, id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public IEnumerable<Recipe> GetAll()
        {
            return _collection.Find(FilterDefinition<Recipe>.Empty).ToList();
        }

        public void Update(Recipe entity)
        {
            var filter = Builders<Recipe>.Filter.Eq(d => d.Id, entity.Id);
            var update = Builders<Recipe>.Update
                .Set(r => r.DishId, entity.DishId)
                .Set(r => r.IngredientId, entity.IngredientId)
                .Set(r => r.Quantity, entity.Quantity)
                .Set(r => r.Unit, entity.Unit);

            _collection.UpdateOne(filter, update);
        }

        private int GenerateNextId()
        {
            var lastRecipe = _collection
                .Find(FilterDefinition<Recipe>.Empty)
                .SortByDescending(d => d.Id) // самый большой Id (последний)
                .FirstOrDefault();

            return lastRecipe == null ? 1 : lastRecipe.Id + 1;
        }
    }
}