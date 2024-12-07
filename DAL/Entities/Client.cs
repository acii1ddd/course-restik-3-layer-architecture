using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities
{
    public class Client
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("login")]
        public string Login { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
    }
}