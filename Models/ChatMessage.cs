namespace BSBoilerPlate.Models
{
    public class ChatMessage
    {
        public long ID { get; set; }

        public int? UserFromID { get; set; }

        public User UserFrom { get; set; }

        public int? UserToID { get; set; }

        public User UserTo { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime? DateTimeRead { get; set; }

        public string Message { get; set; }
    }
}
