using BLL.ServiceInterfaces;
using DAL.Entities;

namespace BLL.DTO
{
    public class OrderDTO : IDTO
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public DateTime Date { get; set; } = new DateTime();

        public decimal? TotalCost { get; set; } // триггер

        public OrderStatus Status { get; set; } = OrderStatus.InProcessing;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public int? WaiterId { get; set; }

        public int? CookId { get; set; }

        public int TableNumber { get; set; }
    }
}
