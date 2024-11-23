using BLL.ServiceInterfaces;

namespace BLL.DTO
{
    public class WorkerDTO : IDTO, IUserDTO
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string Login { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime? HireDate { get; set; } = new DateTime();

        public string FullName { get; set; } = string.Empty;
    }
}
