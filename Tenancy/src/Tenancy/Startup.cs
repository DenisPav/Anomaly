using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenancy.Common.DI;
using Tenancy.Configuration;
using Tenancy.Db;
using Tenancy.Middlewares;
using Swashbuckle.AspNetCore.Swagger;
using Tenancy.Services.Middlewares;

namespace Tenancy
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public IHostingEnvironment Env { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AppOptions>(Configuration);
            services.AddDbContext<DatabaseContext>(opts => opts.UseSqlServer(Configuration["db"]));
            services.Scan(
                scan =>
                {
                    scan.FromAssemblyDependencies(typeof(Startup).Assembly)
                        .AddClasses(classes => classes.WithAttribute<ScopedServiceAttribute>())
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()

                        .AddClasses(classes => classes.WithAttribute<TransientServiceAttribute>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()

                        .AddClasses(classes => classes.WithAttribute<SingletonServiceAttribute>())
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime();
                }
            );
            services.AddMvc(opts => opts.AllowEmptyInputInBodyModelBinding = true);
            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new Info
                {
                    Title = "Api title",
                    Version = "1"
                });
            });
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (Env.IsProduction())
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
            }

            app.UseSwagger();
            app.UseSwaggerUI(opts =>
            {
                opts.RoutePrefix = Configuration["swaggerRoute"];
                opts.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
            });
            app.UseMiddleware<TenantResolveMiddleware>();
            app.UseMvc();
        }
    }
}
