using BLL.ServiceInterfaces.DTOs;

namespace BLL.DTO
{
    public class DishDTO : IDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DishDTO dTO &&
                   Id == dTO.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
