using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Versioning.Conventions;

namespace Versioning
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opts =>
            {
                opts.Conventions.Add(new HeaderControllerVersionConvention());
                opts.Conventions.Add(new HeaderActionVersionConvention());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }
}
