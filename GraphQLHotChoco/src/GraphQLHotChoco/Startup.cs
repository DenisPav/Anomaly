using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQLHotChoco.Database;
using GraphQLHotChoco.Models;
using GraphQLHotChoco.Queries;
using HotChocolate;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQLHotChoco
{
    public class Startup
    {
        readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<DatabaseContext>(opts => opts.UseSqlServer(Configuration["db"]).EnableSensitiveDataLogging(true));

            

            services.AddGraphQL(sp => Schema.Create(opts =>
            {
                opts.RegisterServiceProvider(sp);

                opts.RegisterQueryType<RootQuery>();
                opts.RegisterType<RootQueryObjectType>();
                opts.RegisterType<UserObjectType>();
            }));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceScopeFactory fact)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseGraphQL("/graphql");
            app.UsePlayground("/graphql", "/playground");
        }
    }
}
