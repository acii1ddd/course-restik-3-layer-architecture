using BLL.ServiceInterfaces;

namespace BLL.DTO
{
    public class DishDTO : IDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }
    }
}
