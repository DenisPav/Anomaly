using ApiSamples.Domain;
using Microsoft.EntityFrameworkCore;

namespace ApiSamples.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var candidate = builder.Entity<Candidate>();
            candidate.HasKey(x => x.Id);
            candidate.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            candidate.Property(x => x.Name)
                .IsRequired();
            candidate.Property(x => x.Surname)
                .IsRequired();

            candidate.HasMany(x => x.Matches)
                .WithOne(x => x.Candidate);

            var position = builder.Entity<Position>();
            position.HasKey(x => x.Id);
            position.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            position.Property(x => x.Name)
                .IsRequired();

            position.HasOne(x => x.Company)
                .WithMany(x => x.Positions);

            var match = builder.Entity<Match>();
            match.HasKey(x => x.Id);
            match.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            match.HasOne(x => x.Position)
                .WithMany(x => x.Matches);

            match.HasOne(x => x.Candidate)
                .WithMany(x => x.Matches);

            var company = builder.Entity<Company>();
            company.HasKey(x => x.Id);
            company.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            company.Property(x => x.Name)
                .IsRequired();
        }
    }
}