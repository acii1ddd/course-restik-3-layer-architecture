namespace DAL.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public UnitsOfMeasurement Unit { get; set; }

        public decimal StockQuantity { get; set; }

        public decimal ThresholdLevel { get; set; }
    }
}

public enum UnitsOfMeasurement
{
    Kg,
    Gram,
    Liter,
    Milliliter
}