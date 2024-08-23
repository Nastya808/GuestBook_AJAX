namespace GuestBookApp.Models
{
    public class IndexModel
    {
        public IEnumerable<Message> Messages { get; set; } = new List<Message>();
        public string UserName { get; set; }
    }
}
