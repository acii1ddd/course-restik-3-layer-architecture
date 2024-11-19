namespace DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public PaymentMethod PaymentMethod { get; set; }

        public int OrderId { get; set; }
    }

    public enum PaymentMethod
    {
        Cash,
        Card
    }
}
