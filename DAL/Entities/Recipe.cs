using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities
{
    public class Recipe
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("dish_id")]
        public int DishId { get; set; }

        [BsonElement("ingredient_id")]
        public int IngredientId { get; set; }

        [BsonElement("quantity")]
        public decimal Quantity { get; set; }

        [BsonElement("unit")]
        public UnitsOfMeasurement Unit { get; set; }
    }
}