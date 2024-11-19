namespace DAL.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public IngredientUnit Unit { get; set; }

        public decimal StockQuantity { get; set; }

        public decimal ThresholdLevel { get; set; }
    }
}

public enum IngredientUnit
{
    Kg,
    Gram,
    Litre,
    Piece
}