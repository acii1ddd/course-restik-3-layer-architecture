using BLL.ServiceInterfaces;

namespace BLL.DTO
{
    public class OrderItemDTO : IDTO
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int DishId { get; set; }

        public int Quantity { get; set; }

        public decimal CurrDishPrice { get; set; }

        public decimal TotalDishPrice { get; set; } // триггер
    }
}
