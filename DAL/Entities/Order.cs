namespace DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public decimal TotalCost { get; set; } // триггер

        public OrderStatus Status { get; set; } = OrderStatus.InProcessing;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public int WaiterId { get; set; }

        public int CookId { get; set; }  

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
