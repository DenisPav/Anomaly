using Microsoft.EntityFrameworkCore;
using SocketLogin.Models;

namespace SocketLogin.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        { }
    }
}
