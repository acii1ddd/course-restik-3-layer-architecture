using DAL.Entities;
using DAL.Interfaces;
using Npgsql;

namespace DAL.PostgresRepositories
{
    internal class WorkerRepository : IWorkerRepository
    {
        private readonly string _connectionString;

        public WorkerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Worker entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO workers (role_id, login, password, phone_number, hire_date, full_name) " +
                    "values (@role_id, @login, @password, @phone_number, @hire_date, @full_name) RETURNING id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@role_id", entity.RoleId);
                    command.Parameters.AddWithValue("@login", entity.Login);
                    command.Parameters.AddWithValue("@password", entity.Password);
                    command.Parameters.AddWithValue("@phone_number", entity.PhoneNumber);
                    command.Parameters.AddWithValue("@hire_date", entity.HireDate.ToDateTime(TimeOnly.MinValue));
                    command.Parameters.AddWithValue("@full_name", entity.FullName);

                    var id = Convert.ToInt32(command.ExecuteScalar()); // так как returnings id
                    entity.Id = id; // id в бд соответствует id объекта
                }
            }
        }

        public void Delete(Worker entity)
        {
            throw new NotImplementedException();
        }

        public Worker? Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Worker> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Worker entity)
        {
            throw new NotImplementedException();
        }
    }
}
