using BLL.ServiceInterfaces.DTOs;
using DAL.Entities;

namespace BLL.DTO
{
    public class PaymentDTO : IDTO
    {
        public int Id { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public PaymentMethod PaymentMethod { get; set; }

        public int OrderId { get; set; }
    }
}
