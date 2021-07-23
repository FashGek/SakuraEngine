namespace Sakura.Service
{
    using Dapr.Client;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using System.Threading;
    using System.Threading.Tasks;

    public class CloudService
    {
        protected CloudService()
        {

        }

        public static T Invoke<T>(string Application, string Scope, object Parameters) 
        {
            var task = InvokeAsync<T>(Application, Scope, Parameters);
            return task.Result;
        }

        public static async ValueTask<T> InvokeAsync<T>(string Application, string Scope, object Parameters)
        {
            var cts = new CancellationTokenSource();
            using var client = new DaprClientBuilder().Build();
            var RV = await client.InvokeMethodAsync<object, T>(Application, Scope, Parameters, cts.Token);
            return RV;
        }

        public static CloudService Run<T>(string[] args) where T : new ()
        {
            CloudService cs = new CloudService();
            cs.CreateHostBuilder<T>(args).Build().Run();
            return cs;
        }

        protected IHostBuilder CreateHostBuilder<T>(string[] args) where T : new() =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<ServiceStartup<T>>();
            });
    }
}
