namespace GuestBookApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int Id_User { get; set; }
        public string? MessageText { get; set; }
        public DateTime MessageDate { get; set; }

        public User User { get; set; } = new User();
    }
}
