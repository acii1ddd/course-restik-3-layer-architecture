namespace DAL.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}