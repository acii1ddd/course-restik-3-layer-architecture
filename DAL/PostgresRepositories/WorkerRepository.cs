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

        // добавление без даты (current date by default в бд)
        public void Add(Worker entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO workers (role_id, login, password, phone_number, full_name) " +
                    "values (@role_id, @login, @password, @phone_number, @full_name) RETURNING id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@role_id", entity.RoleId);
                    command.Parameters.AddWithValue("@login", entity.Login);
                    command.Parameters.AddWithValue("@password", entity.Password);
                    command.Parameters.AddWithValue("@phone_number", entity.PhoneNumber);
                    command.Parameters.AddWithValue("@full_name", entity.FullName);

                    var id = Convert.ToInt32(command.ExecuteScalar()); // так как returnings id
                    entity.Id = id; // id в бд соответствует id объекта
                }
            }
        }

        public void Delete(Worker entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM workers WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.ExecuteNonQuery(); // выполн удаление
                }
            }
        }

        public Worker? Get(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM workers WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Worker
                            {
                                Id = reader.GetInt32(0),
                                RoleId = reader.GetInt32(1),
                                Login = reader.GetString(2),
                                Password = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                HireDate = reader.GetDateTime(5),
                                FullName = reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return null; // если не найден
        }

        public IEnumerable<Worker> GetAll()
        {
            var workers = new List<Worker>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM workers";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            workers.Add(new Worker
                            {
                                Id = reader.GetInt32(0),
                                RoleId = reader.GetInt32(1),
                                Login = reader.GetString(2),
                                Password = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                HireDate = reader.GetDateTime(5),
                                FullName = reader.GetString(6)
                            });
                        }
                    }
                }
                return workers; // пустой список new List<Worker>(), либо список клиентов
            }
        }

        public Worker? GetByLogin(string login)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM workers WHERE login = @login";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Worker
                            {
                                Id = reader.GetInt32(0),
                                RoleId = reader.GetInt32(1),
                                Login = reader.GetString(2),
                                Password = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                HireDate = reader.GetDateTime(5),
                                FullName = reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return null; // если не найден
        }

        public void Update(Worker entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE workers SET " +
                            "role_id = @role_id, " +
                            "login = @login, " +
                            "password = @password, " +
                            "phone_number = @phone_number, " +
                            "full_name = @full_name " +
                            "WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@role_id", entity.RoleId);
                    command.Parameters.AddWithValue("@login", entity.Login);
                    command.Parameters.AddWithValue("@password", entity.Password);
                    command.Parameters.AddWithValue("@phone_number", entity.PhoneNumber);
                    command.Parameters.AddWithValue("@full_name", entity.FullName);
                    command.Parameters.AddWithValue("@id", entity.Id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
