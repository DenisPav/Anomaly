using MagicLinks.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MagicLinks.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        { }

        public async Task CreateUsers()
        {
            var users = new[] {
                new User { Email = "first@gmail.com" },
                new User { Email = "second@gmail.com" }
            };

            await this.Users.AddRangeAsync(users);
            await this.SaveChangesAsync();
        }
    }
}
