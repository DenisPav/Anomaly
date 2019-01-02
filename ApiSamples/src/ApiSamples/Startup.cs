using ApiSamples.Config;
using ApiSamples.Database;
using ApiSamples.Domain;
using ApiSamples.Queries;
using ApiSamples.Services;
using AutoMapper;
using HotChocolate;
using HotChocolate.AspNetCore.GraphiQL;
using HotChocolate.AspNetCore.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Services;

namespace ApiSamples
{
    public class Startup
    {
        readonly IConfiguration Configuration;

        public Startup(
            IConfiguration configuration
        )
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(
                opts => opts.UseSqlServer(Configuration["Db"])
            );
            services.AddMvc()
                .AddJsonOptions(opts =>
                {
                    opts.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddSingleton(AutoMapperConfig.GetConfiguration() as AutoMapper.IConfigurationProvider);
            services.AddScoped(s => new Mapper(s.GetRequiredService<AutoMapper.IConfigurationProvider>(), s.GetRequiredService) as IMapper);

            services.AddSwaggerGen(opts => opts.SwaggerDoc("api-sample", new Swashbuckle.AspNetCore.Swagger.Info
            {
                Title = "Api Examples",
                Version = "1"
            }));

            services.AddScoped<QueryType>();
            services.AddScoped<ISieveProcessor, CustomSieveProcessor>();
            services.AddQueryProvider<DatabaseContext>(opts =>
            {
                opts.Configure<Candidate>(candidate =>
                {
                    candidate.Map("Id", mapping => mapping.Id);
                    candidate.Map("Name", mapping => mapping.Name);
                    candidate.Map("Surname", mapping => mapping.Surname);
                });

                opts.Configure<Match>(match =>
                {
                    match.Map("Id", mapping => mapping.Id);
                });

                opts.Configure<Position>(position =>
                {
                    position.Map("Id", mapping => mapping.Id);
                    position.Map("Name", mapping => mapping.Name);
                    position.Map("Openings", mapping => mapping.Openings);
                });

                opts.Configure<Company>(company =>
                {
                    company.Map("Id", mapping => mapping.Id);
                    company.Map("Name", mapping => mapping.Name);
                    company.Map("Position", mapping => mapping.Positions, position => position.Name);
                });
            });

            services.AddGraphQL(sp => Schema.Create(opts =>
            {
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
