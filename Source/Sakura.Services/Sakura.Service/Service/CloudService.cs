namespace Sakura.Service
{
    using Dapr.Client;
    using System.Linq;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using System.Threading;
    using System.Threading.Tasks;

    public class CloudService
    {
        protected CloudService() {}
        public static T Invoke<T>(string Application, string Scope, object Parameters) 
        {
            var task = InvokeAsync<T>(Application, Scope, Parameters);
            return task.Result;
        }

        public static async ValueTask<T> InvokeAsync<T>(string Application, string Scope, object Parameters)
        {
            var cts = new CancellationTokenSource();
            using var client = new DaprClientBuilder().UseJsonSerializationOptions(
                new System.Text.Json.JsonSerializerOptions
                {
                    DictionaryKeyPolicy = ServiceJsonNamingPolicy.Policy,
                    PropertyNameCaseInsensitive = false
                }
                ).Build();
            try
            {
                return await client.InvokeMethodAsync<object, T>(Application, Scope, Parameters, cts.Token);
            }
            catch (InvocationException E)
            {
                string Content = await E.Response.Content.ReadAsStringAsync();
                System.Console.WriteLine($"Invoke {E.AppId}::{E.MethodName} Error.\n" +
                    $" Status Code: {E.Response.StatusCode}\n" +
                    $" Content: {Content}");
                return default(T);
            }
        }

        public static CloudService Run<T>(string[] args) where T : new ()
        {
            CloudService cs = new CloudService();
            cs.CreateHostBuilder<T>(args).Build().Run();
            return cs;
        }

        public static ServiceInstance WaitUntilServiceStarted(string Application)
        {
            ServiceInstance AssetServiceInstance = null;
            while (AssetServiceInstance is null)
            {
                var DaprList2 = DaprCLI.DaprList().Result;
                var AssetServiceExisted2 = DaprList2 is null ? null :
                                        from Dapr in DaprList2
                                        where Dapr.appId == Application
                                        select Dapr;
                if (AssetServiceExisted2 is not null)
                AssetServiceInstance = AssetServiceExisted2.Any()? AssetServiceExisted2?.ElementAt(0) : null;
            }
            return AssetServiceInstance;
        }

        protected IHostBuilder CreateHostBuilder<T>(string[] args) where T : new() =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime(opts => opts.SuppressStatusMessages = true)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<DaprServiceStartup<T>>();
                });
    }
}
