using BLL.ServiceInterfaces.DTOs;

namespace BLL.DTO
{
    public class RecipeDTO : IDTO
    {
        public int Id { get; set; }

        public int DishId { get; set; }

        public int IngredientId { get; set; }

        public decimal Quantity { get; set; }

        public RecipeUnit Unit { get; set; }
    }
}
