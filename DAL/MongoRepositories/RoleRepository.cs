using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class RoleRepository : IRoleRepository
    {
        private readonly IMongoCollection<Role> _collection;

        public RoleRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Role>(collectionName);
        }

        public void Add(Role entity)
        {
            if (entity.Id == 0) // Если Id не задан
            {
                entity.Id = GenerateNextId();
            }
            _collection.InsertOne(entity);
        }

        public void Delete(Role entity)
        {
            var filter = Builders<Role>.Filter.Eq(d => d.Id, entity.Id);
            _collection.DeleteOne(filter);
        }

        public Role? Get(int id)
        {
            var filter = Builders<Role>.Filter.Eq(d => d.Id, id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public IEnumerable<Role> GetAll()
        {
            return _collection.Find(FilterDefinition<Role>.Empty).ToList();
        }

        public void Update(Role entity)
        {
            var filter = Builders<Role>.Filter.Eq(d => d.Id, entity.Id);
            var update = Builders<Role>.Update
                .Set(r => r.Name, entity.Name);

            _collection.UpdateOne(filter, update);
        }

        private int GenerateNextId()
        {
            var lastRole = _collection
                .Find(FilterDefinition<Role>.Empty)
                .SortByDescending(d => d.Id) // самый большой Id (последний)
                .FirstOrDefault();

            return lastRole == null ? 1 : lastRole.Id + 1;
        }
    }
}
