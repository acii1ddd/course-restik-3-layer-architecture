namespace DAL.Entities
{
    public class Worker
    {
        public int Id { get; set; }

        public int RoleId { get; set; }
        
        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateOnly HireDate { get; set; } = new DateOnly();

        public string FullName { get; set; } = string.Empty;
    }
}
