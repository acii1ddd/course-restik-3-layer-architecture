using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities
{
    public class Order
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("client_id")]
        public int ClientId { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [BsonElement("total_cost")]
        public decimal? TotalCost { get; set; } // триггер

        [BsonElement("status")]
        public OrderStatus Status { get; set; } = OrderStatus.InProcessing;

        [BsonElement("payment_status")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        [BsonElement("waiter_id")]
        public int? WaiterId { get; set; }

        [BsonElement("cook_id")]
        public int? CookId { get; set; }

        [BsonElement("table_number")]
        public int TableNumber { get; set; }
    }

    public enum OrderStatus
    {
        InProcessing,
        IsCooking,
        Cooked,
        InDelivery,
        Delivered,
        Completed
    }

    public enum PaymentStatus
    {
        Unpaid,
        Paid
    }
}
