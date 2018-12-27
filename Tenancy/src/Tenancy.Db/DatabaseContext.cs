using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Tenancy.Models;

namespace Tenancy.Db
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        public Tenant ActiveTenant { get; private set; }

        public DatabaseContext(DbContextOptions options) : base(options) { }

        public void SetActiveTenant(Tenant tenant) => ActiveTenant = tenant;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var tenant = modelBuilder.Entity<Tenant>();

            tenant.Property(x => x.Host).IsRequired();
            tenant.HasKey(x => x.Id);
            tenant.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            var user = modelBuilder.Entity<User>();

            user.HasIndex(x => x.Email)
                .IsUnique();

            user.HasKey(x => x.Id);
            user.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            user.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .IsRequired();

            user.HasQueryFilter(x => x.TenantId == ActiveTenant.Id);

            var tenants = new[] {
                new Tenant
                {
                    Id = 1,
                    Host = "localhost:5000"
                },
                new Tenant
                {
                    Id = 2,
                    Host = "localhost:5001"
                }
            };

            tenant.HasData(tenants);

            var faker = new Faker();

            var users = new[] {
                new User
                {
                    Id = 1,
                    TenantId = 1,
                    Email = faker.Internet.Email(faker.Name.FirstName(), faker.Name.LastName())
                },
                new User
                {
                    Id = 2,
                    TenantId = 2,
                    Email = faker.Internet.Email(faker.Name.FirstName(), faker.Name.LastName())
                },
                new User
                {
                    Id = 3,
                    TenantId = 1,
                    Email = faker.Internet.Email(faker.Name.FirstName(), faker.Name.LastName())
                },
                new User
                {
                    Id = 4,
                    TenantId = 2,
                    Email = faker.Internet.Email(faker.Name.FirstName(), faker.Name.LastName())
                },
                new User
                {
                    Id = 5,
                    TenantId = 1,
                    Email = faker.Internet.Email(faker.Name.FirstName(), faker.Name.LastName())
                }
            };

            user.HasData(users);

            base.OnModelCreating(modelBuilder);
        }
    }
}