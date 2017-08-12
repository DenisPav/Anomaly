using SocketLogin.Database;

namespace SocketLogin.Extensions
{
    public static class DatabaseContextExtensions
    {
        public static void EnsureCreated(this DatabaseContext db)
        {
            db.Users.AddRange(new Models.User
            {
                Email = "first@gmail.com"
            });

            db.SaveChanges();
        }
    }
}
