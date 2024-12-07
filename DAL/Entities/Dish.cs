using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities
{
    public class Dish
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("is_available")]
        public bool IsAvailable { get; set; } = true;
    }
}
