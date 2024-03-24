using BSBoilerPlate.Models;
using Microsoft.EntityFrameworkCore;

namespace BSBoilerPlate.Data
{
    public class BSBoilerPlateContext : DbContext
    {
        public BSBoilerPlateContext(DbContextOptions<BSBoilerPlateContext> options) : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<LogAction> LogActions { get; set; }
        public DbSet<LogItem> LogItems { get; set; }
    }
}
