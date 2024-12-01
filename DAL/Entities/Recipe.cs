namespace DAL.Entities
{
    public class Recipe
    {
        public int Id { get; set; }

        public int DishId { get; set; }

        public int IngredientId { get; set; }

        public decimal Quantity { get; set; }

        public UnitsOfMeasurement Unit { get; set; }
    }
}