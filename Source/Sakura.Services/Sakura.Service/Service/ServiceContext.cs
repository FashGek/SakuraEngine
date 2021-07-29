namespace Sakura.Service
{
    using System;
    using System.Threading.Tasks;
    using Dapr.Client;
    using Microsoft.AspNetCore.Http;

    public interface IServiceContext
    {
        public Task<T> GetStateAsync<T>(string StoreName, string Key);
        public Task SaveStateAsync<T>(string StoreName, string Key, T Value);
    }

    public class ServiceContext : IServiceContext
    {
        public ServiceContext(DaprClient client, HttpContext context)
        {
            this.daprClient = client;
            this.httpContext = context;
        }

        public async Task<T> GetStateAsync<T>(string StoreName, string Key)
        {
            return await daprClient.GetStateAsync<T>(StoreName, Key);
        }

        public async Task SaveStateAsync<T>(string StoreName, string Key, T Value)
        {
            await daprClient.SaveStateAsync(StoreName, Key, Value);
        }
        protected DaprClient daprClient { get; }
        public HttpContext httpContext { get; }
    }
}
