namespace BobaTeaFace.Models
{
    public class Chat
    {
        public int Id { get; set; }

        public string SessionId { get; set; }

        public string Role { get; set; }
        public string Content { get; set; }

        public DateTime DateTimeCreated { get; set; }
    }
}
