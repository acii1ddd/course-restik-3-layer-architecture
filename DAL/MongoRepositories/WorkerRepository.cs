using DAL.Entities;
using DAL.Interfaces;
using MongoDB.Driver;

namespace DAL.MongoRepositories
{
    internal class WorkerRepository : IWorkerRepository
    {
        private readonly IMongoCollection<Worker> _collection;

        public WorkerRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Worker>(collectionName);
        }

        public void Add(Worker entity)
        {
            if (entity.Id == 0) // Если Id не задан
            {
                entity.Id = GenerateNextId();
            }
            _collection.InsertOne(entity);
        }

        public void Delete(Worker entity)
        {
            var filter = Builders<Worker>.Filter.Eq(d => d.Id, entity.Id);
            _collection.DeleteOne(filter);
        }

        public Worker? Get(int id)
        {
            var filter = Builders<Worker>.Filter.Eq(d => d.Id, id);
            var worker = _collection.Find(filter).FirstOrDefault();
            if (worker != null)
            {
                // преобразуем дату из UTC в локальный часовой пояс
                worker.HireDate = ConvertToLocal(worker.HireDate);
            }
            return worker;
        }

        public IEnumerable<Worker> GetAll()
        {
            var workers = _collection.Find(FilterDefinition<Worker>.Empty).ToList();
            foreach (var worker in workers)
            {
                // Преобразуем дату из UTC в локальный часовой пояс
                worker.HireDate = ConvertToLocal(worker.HireDate);
            }
            return workers;
        }

        public Worker? GetByLogin(string login)
        {
            var filter = Builders<Worker>.Filter.Eq(c => c.Login, login); // получать по login = переданному login
            var worker = _collection.Find(filter).FirstOrDefault();
            if (worker != null)
            {
                // преобразуем дату из UTC в локальный часовой пояс
                worker.HireDate = ConvertToLocal(worker.HireDate);
            }
            return worker;
        }

        public void Update(Worker entity)
        {
            var filter = Builders<Worker>.Filter.Eq(d => d.Id, entity.Id);
            var update = Builders<Worker>.Update
                .Set(w => w.RoleId, entity.RoleId)
                .Set(w => w.Login, entity.Login)
                .Set(w => w.Password, entity.Password)
                .Set(w => w.PhoneNumber, entity.PhoneNumber)
                .Set(w => w.HireDate, entity.HireDate)
                .Set(w => w.FullName, entity.FullName);

            _collection.UpdateOne(filter, update);
        }

        private int GenerateNextId()
        {
            var lastWorker = _collection
                .Find(FilterDefinition<Worker>.Empty)
                .SortByDescending(d => d.Id) // самый большой Id (последний)
                .FirstOrDefault();

            return lastWorker == null ? 1 : lastWorker.Id + 1;
        }

        private DateTime ConvertToLocal(DateTime utcDate)
        {
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Minsk"); // Минск
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, localTimeZone);
        }
    }
}
