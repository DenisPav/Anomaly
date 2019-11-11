using Bogus;
using Microsoft.EntityFrameworkCore;
using ParserSample.Models;
using System;
using System.Linq;

namespace ParserSample.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Startup).Assembly);
        }
    }

    public static class DatabaseContextExtensions
    {
        public static void Seed(this DatabaseContext db)
        {
            var posts = db.Set<Post>();

            if (!posts.Any())
            {
                var postsFaker = new Faker<Post>()
                    .RuleFor(post => post.Id, () => Guid.NewGuid())
                    .RuleFor(post => post.CreationDate, f => f.Date.Recent(300))
                    .RuleFor(post => post.Count, f => f.Random.Int())
                    .RuleFor(post => post.Name, f => f.Internet.DomainName());

                var generatedPosts = postsFaker.Generate(100);

                posts.AddRange(generatedPosts);
                db.SaveChanges();
            }
        }
    }
}
