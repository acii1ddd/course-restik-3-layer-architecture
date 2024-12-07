using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities
{
    public class OrderItem
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("order_id")]
        public int OrderId { get; set; }

        [BsonElement("dish_id")]
        public int DishId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("curr_dish_price")]
        public decimal CurrDishPrice { get; set; }

        [BsonElement("total_dish_price")]
        public decimal TotalDishPrice { get; set; } // триггер
    }
}
