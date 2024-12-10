using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities
{
    public class Worker
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("role_id")]
        public int RoleId { get; set; }

        [BsonElement("login")]
        public string Login { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [BsonElement("hire_date")]
        public DateTime HireDate { get; set; } = DateTime.Now; // optional, current date by default

        [BsonElement("full_name")]
        public string FullName { get; set; } = string.Empty;
    }
}