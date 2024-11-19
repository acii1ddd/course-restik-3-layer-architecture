using DAL.Entities;
using DAL.Interfaces;
using Npgsql;

namespace DAL.PostgresRepositories
{
    internal class ClientRepository:IClientRepository
    {
        private readonly string _connectionString; // можно изменить значение read-only переменной ни в каком другом методе, кроме конструктора

        public ClientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Добавляет клинета в таблицу clients 
        /// и устанавливает его Id на основе autoincrement Id полученного из базы данных
        /// </summary>
        /// <param name="entity">Объект клиента, который нужно добавить</param>
        public void Add(Client entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO clients (login, password, name) values (@login, @password, @name) RETURNING id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", entity.Login);
                    command.Parameters.AddWithValue("@password", entity.Password);
                    command.Parameters.AddWithValue("@name", entity.Name);

                    var id = Convert.ToInt32(command.ExecuteScalar()); // так как returnings id
                    entity.Id = id; // id в бд соответствует id объекта
                }
            }
        }

        public void Delete(Client entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM clients WHERE id = @id";
                
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.ExecuteNonQuery(); // выполн удаление
                }
            }
        }

        public Client? Get(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT id, login, password, name FROM clients WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Client
                            {
                                Id = reader.GetInt32(0),
                                Login = reader.GetString(1),
                                Password = reader.GetString(2),
                                Name = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null; // если не найден
        }

        public IEnumerable<Client> GetAll()
        {
            var clients = new List<Client>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM clients";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clients.Add(new Client
                            {
                                Id = reader.GetInt32(0),
                                Login = reader.GetString(1),
                                Password = reader.GetString(2),
                                Name = reader.GetString(3)
                            });
                        }
                    }
                }
                return clients; // пустой список new List<Client>(), либо список клиентов
            }
        }

        // принимает новую сущность и обновляет с ее помощью старую (с таким же id)
        public void Update(Client entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE clients SET " +
                            "login = @login, " +
                            "password = @password, " +
                            "name = @name " +
                            "WHERE id = @id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", entity.Login);
                    command.Parameters.AddWithValue("@password", entity.Password);
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@id", entity.Id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
