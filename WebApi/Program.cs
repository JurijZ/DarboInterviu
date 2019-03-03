using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Main started");

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .UseNLog()  // NLog: setup NLog for Dependency injection
                .UseUrls("http://localhost:62894")
                .Build();
    }
}
