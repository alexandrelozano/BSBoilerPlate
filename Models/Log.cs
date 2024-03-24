namespace BSBoilerPlate.Models
{
    public class Log
    {
        public long ID { get; set; }

        public int LogActionID { get; set; }

        public LogAction LogAction { get; set; }

        public DateTime DateTime { get; set; }

        public int UserID { get; set; }

        public User User { get; set; }

        public virtual ICollection<LogItem> LogItems { get; set; }
    }
}
