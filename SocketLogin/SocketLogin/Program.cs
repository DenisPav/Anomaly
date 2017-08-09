using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SocketLogin
{
    public class Program
    {
        public const string URL = "http://localhost:5000";

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls(URL)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
