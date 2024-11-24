namespace DAL.Entities
{
    public class Worker
    {
        public int Id { get; set; }

        public int RoleId { get; set; }
        
        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime HireDate { get; set; } = DateTime.Now.Date; // optional, current date by default

        public string FullName { get; set; } = string.Empty;
    }
}
