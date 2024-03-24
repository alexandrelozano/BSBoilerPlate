using System.Linq.Expressions;

namespace BSBoilerPlate.Models.DTOs
{
    public class LogListItemDto
    {
        public long ID { get; set; }
        public DateTime DateTime { get; set; }
        public string LogAction { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public ICollection<LogItem> LogItems { get; set; }
    }
}
