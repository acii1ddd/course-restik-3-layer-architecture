using BLL.ServiceInterfaces.DTOs;

namespace BLL.DTO
{
    public class ClientDTO : IDTO, IUserDTO
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
