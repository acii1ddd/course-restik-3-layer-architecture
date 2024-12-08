using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class ClientRepository : IClientRepository
    {
        private readonly IMongoCollection<Client> _collection;

        public ClientRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Client>(collectionName);
        }

        public void Add(Client entity)
        {
            // Если Id = 0, то сгенерируем новый уникальный Id
            if (entity.Id == 0)
            {
                entity.Id = GenerateNextId();
            }
            _collection.InsertOne(entity);
        }

        public void Delete(Client entity)
        {
            var filter = Builders<Client>.Filter.Eq(c => c.Id, entity.Id); // удалять с id = entity.Id
            _collection.DeleteOne(filter);
        }

        public Client? Get(int id)
        {
            var filter = Builders<Client>.Filter.Eq(c => c.Id, id); // получать по id который передан в Get
            return _collection.Find(filter).FirstOrDefault();
        }

        public IEnumerable<Client> GetAll()
        {
            return _collection.Find(FilterDefinition<Client>.Empty).ToList(); // {} - получить всех
        }

        public Client? GetByLogin(string login)
        {
            var filter = Builders<Client>.Filter.Eq(c => c.Login, login); // получать по login = переданному login
            return _collection.Find(filter).FirstOrDefault();
        }

        public void Update(Client entity)
        {
            var filter = Builders<Client>.Filter.Eq(c => c.Id, entity.Id); // ищем по id = entity.Id элемент для изменения
            var update = Builders<Client>.Update // построение операции обновления
                .Set(c => c.Login, entity.Login) // Login будет обновлен на entity.Login
                .Set(c => c.Password, entity.Password)
                .Set(c => c.Name, entity.Name);

            _collection.UpdateOne(filter, update); // filter - какой документ нужно обновить / update - какие изменения внести
        }

        private int GenerateNextId()
        {
            var lastClient = _collection
                .Find(FilterDefinition<Client>.Empty) // получаем всех
                .SortByDescending(c => c.Id)
                .FirstOrDefault();

            return lastClient == null ? 1 : lastClient.Id + 1;
        }
    }
}
