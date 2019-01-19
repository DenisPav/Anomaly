using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace TraefikSampleSecond
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/update", nested =>
            {
                nested.Run(async ctx =>
                {
                    await "http://localhost:8080/api/providers/rest".AllowAnyHttpStatus()
                    .PutJsonAsync(new
                    {
                        backends = new
                        {
                            backend1 = new
                            {
                                servers = new
                                {
                                    server0 = new
                                    {
                                        url = "http://localhost:5001",
                                        weight = 1
                                    },
                                    server1 = new
                                    {
                                        url = "http://localhost:5003",
                                        weight = 2
                                    }
                                },
                            },
                            backend2 = new
                            {
                                servers = new
                                {
                                    server0 = new
                                    {
                                        url = "http://localhost:5002",
                                        weight = 1
                                    }
                                },
                            }
                        },
                        frontends = new
                        {
                            frontend2 = new
                            {
                                backend = "backend2",
                                routes = new
                                {
                                    entry = new
                                    {
                                        rule = "Path:/second"
                                    }
                                }
                            }
                        }
                    });

                    await ctx.Response.WriteAsync("Configuration updated");
                });
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync($"Service 2 -> {context.Request.Host.Port}");
            });
        }
    }
}
