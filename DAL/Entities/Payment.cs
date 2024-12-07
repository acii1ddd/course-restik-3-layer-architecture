using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities
{
    public class Payment
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("payment_date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [BsonElement("payment_method")]
        public PaymentMethod PaymentMethod { get; set; }

        [BsonElement("order_id")]
        public int OrderId { get; set; }
    }

    public enum PaymentMethod
    {
        Cash,
        Card
    }
}
