using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Versioning.Attributes;
using Versioning.Conventions;
using Versioning.Swagger;

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

            services.AddSwaggerGen(opts =>
            {
                var provider = services.BuildServiceProvider()
                    .GetRequiredService<IApiDescriptionGroupCollectionProvider>();

                opts.DocumentFilter<HeaderOperationFilter>();

                foreach (var version in GetVersions())
                {
                    opts.SwaggerDoc($"v{version}", new Info
                    {
                        Title = $"Api V{version}",
                        Version = $"v{version}"
                    });
                }
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(opts =>
            {
                foreach (var version in GetVersions())
                    opts.SwaggerEndpoint($"/swagger/v{version}/swagger.json", $"V{version}");
            });
            app.UseMvc();
        }

        private IEnumerable<string> GetVersions()
        {
            return typeof(Startup).Assembly
                .GetTypes()
                .Where(type => type.BaseType != null && type.BaseType == typeof(Controller))
                .SelectMany(type => {
                    var controllerAttributes = type.GetCustomAttributes(true);
                    var actionAttributes = type.GetMethods().SelectMany(method => method.GetCustomAttributes(true));

                    return controllerAttributes.Concat(actionAttributes);
                })
                .OfType<VersionAttribute>()
                .Select(x => x.Version)
                .Distinct()
                .OrderBy(x => x);
        }
    }
}
