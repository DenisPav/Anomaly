using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ParserSample.Database;
using ParserSample.Expressions;
using ParserSample.Filters;
using ParserSample.Models;
using ParserSample.Models.Filters;
using ParserSample.Parsers;

namespace ParserSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite("Data Source=./database.db"));
            services.AddControllers();

            services.AddSingleton(typeof(FilterContainer<>));
            services.AddSingleton<FilterConfiguration<Post>, PostFilterConfiguration>();
            services.AddSingleton(typeof(FilterBuilder<>));
            services.AddSingleton(typeof(IFilterParser<>), typeof(FilterParser<>));
            services.AddScoped(typeof(IExpressionProvider<>), typeof(ExpressionProvider<>));
            services.AddScoped(typeof(IFilterProvider<>), typeof(FilterProvider<>));
        }

        public void Configure(IApplicationBuilder app, IServiceScopeFactory scopeFactory)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            db.Seed();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
