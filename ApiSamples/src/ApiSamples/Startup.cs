using ApiSamples.Config;
using ApiSamples.Database;
using AutoMapper;
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
                .AddJsonOptions(opts => {
                    opts.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddSingleton(AutoMapperConfig.GetConfiguration() as IConfigurationProvider);
            services.AddScoped(s => new Mapper(s.GetRequiredService<IConfigurationProvider>(), s.GetRequiredService) as IMapper);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
