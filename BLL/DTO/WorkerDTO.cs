using BLL.ServiceInterfaces;

namespace BLL.DTO
{
    public class WorkerDTO : IDTO
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string Login { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateOnly HireDate { get; set; } = new DateOnly();

        public string FullName { get; set; } = string.Empty;
    }
}
