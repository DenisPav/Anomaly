using ApiSamples.Config;
using ApiSamples.Database;
using ApiSamples.Queries;
using AutoMapper;
using HotChocolate;
using HotChocolate.AspNetCore.GraphiQL;
using HotChocolate.AspNetCore.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiSamples
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(
                opts => opts.UseSqlServer("")
            );
            services.AddMvc()
                .AddJsonOptions(opts =>
                {
                    opts.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddSingleton(AutoMapperConfig.GetConfiguration() as IConfigurationProvider);
            services.AddScoped(s => new Mapper(s.GetRequiredService<IConfigurationProvider>(), s.GetRequiredService) as IMapper);

            services.AddSwaggerGen(opts => opts.SwaggerDoc("api-sample", new Swashbuckle.AspNetCore.Swagger.Info
            {
                Title = "Api Examples",
                Version = "1"
            }));

            services.AddScoped<QueryType>();

            services.AddGraphQL(sp => Schema.Create(opts => {
                opts.RegisterServiceProvider(sp);

                opts.RegisterQueryType<QueryType>();

                opts.RegisterType<CandidateType>();
                opts.RegisterType<PositionType>();
            }));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
            app.UseGraphQL("/graphql");
            app.UseGraphiQL("/graphql", "/graphiql");
            app.UsePlayground("/graphql", "/playground");

            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/api-sample/swagger.json", "V1"));
        }
    }
}
