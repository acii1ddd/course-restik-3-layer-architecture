using BLL.ServiceInterfaces;

namespace BLL.DTO
{
    public class RoleDTO : IDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
