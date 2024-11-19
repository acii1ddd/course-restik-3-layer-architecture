namespace DAL.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        
        public int DishId { get; set; }

        public int Quantity { get; set; }

        public decimal CurrDishPrice { get; set; }

        public decimal TotalDishPrice { get; set; } // триггер
    }
}
