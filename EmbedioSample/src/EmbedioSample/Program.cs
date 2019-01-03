using EmbedioSample.Controllers;
using EmbedioSample.Services;
using FluentAssemblyScanner;
using Grace.DependencyInjection;
using System;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace EmbedioSample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = new DependencyInjectionContainer())
            {
                container.Configure(config =>
                {
                    config.Export<NoteRepository>().As<INoteRepository>();

                    var controllerTypes = AssemblyScanner.FromThisAssembly()
                        .BasedOn<WebApiController>()
                        .Filter()
                        .Classes()
                        .Scan();

                    config.Export(controllerTypes);
                });

                using (var server = new WebServer(5000))
                {
                    server.RegisterModule(new WebApiModule());
                    server.Module<WebApiModule>().RegisterController<NoteController>(ctx => container.Locate<NoteController>(ctx));

                    server.RunAsync();

                    Console.ReadKey(true);
                }
            }
        }
    }
}
