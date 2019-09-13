using HotChocholateExpandable.Database;
using HotChocholateExpandable.Extensions;
using HotChocholateExpandable.GraphQL;
using HotChocholateExpandable.Models;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

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
            services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite(Config["Db"]).EnableSensitiveDataLogging(true), ServiceLifetime.Scoped);

            services.AddGraphQL(sp => Schema.Create(opts =>
            {
                opts.RegisterServiceProvider(sp);

                opts.RegisterQueryType<RootQuery>();
                opts.RegisterType<RootQueryObject>();
                opts.RegisterType<UserObjectType>();
                opts.RegisterType<BlogObjectType>();
                opts.RegisterType<BlogPostObjectType>();
                opts.RegisterType<CommentObjectType>();
                opts.RegisterType<TagObjectType>();
                //custom api model
                opts.RegisterType<BlogPostApiModelObjectType>();
            }), new QueryExecutionOptions { IncludeExceptionDetails = true });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DatabaseContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            db.Seed();

            app.UseMvc();
            app.UseGraphQL("/graphql");
            app.UsePlayground("/graphql", "/playground");
            app.UseVoyager("/graphql", "/voyager");
        }
    }
}
