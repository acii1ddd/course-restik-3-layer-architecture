using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities
{
    public class Role
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
    }

    public enum WorkerRole
    {
        waiter,
        cook,
        admin,
    }
}