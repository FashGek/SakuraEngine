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
        /// <summary>
        ///     This method trys to invoke with a default service context. 
        ///     For better tracking, use IServiceContext.Invoke if possible.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="Application"></param>
        /// <param name="Scope"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public static TResponse Invoke<TRequest, TResponse>(string Application, string Scope, TRequest Parameters)
            => InvokeAsync<TRequest, TResponse>(Application, Scope, Parameters).Result;
        public static async ValueTask<TResponse> InvokeAsync<TRequest, TResponse>(string Application, string Scope, TRequest Parameters)
            => await Client.InvokeAsync<TRequest, TResponse>(Application, Scope, Parameters, default(CancellationToken));

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
