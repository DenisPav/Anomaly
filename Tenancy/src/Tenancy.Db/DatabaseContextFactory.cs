using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Tenancy.Configuration;

namespace Tenancy.Db
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable(Constants.ASPNETCORE_ENVIRONMENT);
            var opts = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"appsettings.{env}.json", false, false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder().UseSqlServer(opts["db"]);

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
