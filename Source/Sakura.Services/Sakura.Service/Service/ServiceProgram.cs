namespace Sakura.Service
{
    using Dapr.Client;
    using System.Linq;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using System.Threading;
    using System.Threading.Tasks;

    public class ServiceProgram
    {
        public static ServiceProgram Run<T>(string[] args) where T : new ()
        {
            ServiceProgram cs = new ServiceProgram();
            cs.CreateHostBuilder<T>(args).Build().Run();
            return cs;
        }

        protected ServiceProgram() { }

        protected IHostBuilder CreateHostBuilder<T>(string[] args) where T : new() =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime(opts => opts.SuppressStatusMessages = true)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<DaprServiceStartup<T>>();
                });
    }
}
