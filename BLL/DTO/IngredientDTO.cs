using BLL.ServiceInterfaces.DTOs;

namespace BLL.DTO
{
    public class IngredientDTO : IDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
