using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddControllers();

            services.AddSingleton(typeof(FilterContainer<>));
            services.AddSingleton<FilterConfiguration<Post>, PostFilterConfiguration>();
            services.AddSingleton(typeof(FilterBuilder<>));
            services.AddSingleton(typeof(IFilterParser<>), typeof(FilterParser<>));
            services.AddScoped(typeof(IExpressionProvider<>), typeof(ExpressionProvider<>));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
