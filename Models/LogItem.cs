using Microsoft.Extensions.Hosting;

namespace BSBoilerPlate.Models
{
    public class LogItem
    {
        public long ID { get; set; }

        public long LogID { get; set; }

        public Log Log { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
