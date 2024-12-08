using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities
{
    public class Ingredient
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("unit")]
        [BsonRepresentation(BsonType.String)] // сериализация как строка
        public UnitsOfMeasurement Unit { get; set; }

        [BsonElement("stock_quantity")]
        public decimal StockQuantity { get; set; }

        [BsonElement("threshold_level")]
        public decimal ThresholdLevel { get; set; }
    }
}

public enum UnitsOfMeasurement
{
    Kg,
    Gram,
    Liter,
    Milliliter
}