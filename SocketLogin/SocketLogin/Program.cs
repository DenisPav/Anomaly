using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SocketLogin
{
    public class Program
    {
        public const string URL = "http://localhost:5000";

        public static void Main(string[] args)
            => BuildWebHost(args).Run();   

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(URL)
                .Build();
    }
}
