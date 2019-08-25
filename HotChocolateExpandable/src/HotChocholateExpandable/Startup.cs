using HotChocholateExpandable.Database;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocholateExpandable
{
    public class Startup
    {
        readonly IConfiguration Config;

        public Startup(IConfiguration config)
        {
            Config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite(Config["Db"]).EnableSensitiveDataLogging(true));
            services.AddGraphQL(sp => Schema.Create(opts =>
            {
                opts.RegisterServiceProvider(sp);

                //opts.RegisterQueryType<RootQuery>();
                //opts.RegisterType<RootQueryObjectType>();
                //opts.RegisterType<UserObjectType>();
                //opts.RegisterType<UserRolesObjectType>();
                //opts.RegisterType<RoleObjectType>();
            }));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseGraphQL("/graphql");
            app.UsePlayground("/graphql", "/playground");
            app.UseVoyager("/graphql", "/voyager");
        }
    }
}
