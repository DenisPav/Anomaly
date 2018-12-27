using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Tenancy
{
    public class Program
    {
        public static void Main(string[] args)
            => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((hostBuilder, builder) => {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostBuilder.Configuration)
                    .CreateLogger();
            })
            .UseSerilog()
            .UseKestrel(opts => opts.AddServerHeader = false)
            .UseStartup<Startup>();
    }
}
