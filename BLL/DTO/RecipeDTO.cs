using BLL.ServiceInterfaces.DTOs;
using DAL.Entities;

namespace BLL.DTO
{
    public class RecipeDTO : IDTO
    {
        public int Id { get; set; }

        public int DishId { get; set; }

        public int IngredientId { get; set; }

        public decimal Quantity { get; set; }

        public UnitsOfMeasurement Unit { get; set; }

        public DishDTO Dish { get; set; }

        public IngredientDTO Ingredient { get; set; }
    }
}
