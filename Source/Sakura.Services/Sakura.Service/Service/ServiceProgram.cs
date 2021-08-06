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
        public static T Invoke<T>(string Application, string Scope, object Parameters) 
        {
            var task = InvokeAsync<T>(Application, Scope, Parameters);
            return task.Result;
        }

        public static async ValueTask<T> InvokeAsync<T>(string Application, string Scope, object Parameters)
        {
            try
            {
                var cts = new CancellationTokenSource();
                return await Client.InvokeMethodAsync<object, T>(Application, Scope, Parameters, cts.Token);
            }
            catch (InvocationException E)
            {
                if (E.Response is null)
                {
                    System.Console.WriteLine(E);
                    return default(T);
                }
                else
                {
                    string Content = await E.Response.Content.ReadAsStringAsync();
                    System.Console.WriteLine($"Invoke {E.AppId}::{E.MethodName} Error.\n" +
                        $"Status Code: {E.Response.StatusCode}\n" +
                        $"Content: {Content}");
                    return default(T);
                }
            }
        }

        public static ServiceProgram Run<T>(string[] args) where T : new ()
        {
            ServiceProgram cs = new ServiceProgram();
            cs.CreateHostBuilder<T>(args).Build().Run();
            return cs;
        }

        protected ServiceProgram() { }

        protected static IServiceContext Client = new DaprServiceContext();
        protected IHostBuilder CreateHostBuilder<T>(string[] args) where T : new() =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime(opts => opts.SuppressStatusMessages = true)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<DaprServiceStartup<T>>();
                });
    }
}
