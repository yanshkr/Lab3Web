using Microsoft.EntityFrameworkCore;
using Lab3Web.DataBase.Sheets;

namespace Lab3Web.DataBase
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Alarm> Alarms { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
