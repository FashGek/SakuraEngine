namespace Sakura.Service
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Dapr.Client;
    using Microsoft.AspNetCore.Http;

    public interface IServiceContext
    {
        public Task<T> GetStateAsync<T>(string storeName, string key);
        public Task SaveStateAsync<T>(string storeName, string key, T value);
        public Task PublishEventAsync<T>(string pubsubName, string eventName,
            T eventData, CancellationToken cancellationToken = default(CancellationToken));
        public Task PublishEventAsync(string pubsubName, string eventName,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ServiceContext : IServiceContext
    {
        public ServiceContext(DaprClient client, HttpContext context)
        {
            this.daprClient = client;
            this.httpContext = context;
        }
        public async Task PublishEventAsync(string pubsubName, string eventName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await daprClient.PublishEventAsync(pubsubName, eventName, cancellationToken);
        }
        public async Task PublishEventAsync<T>(string pubsubName, string eventName,
            T eventData, CancellationToken cancellationToken = default(CancellationToken))
        {
            await daprClient.PublishEventAsync<T>(pubsubName, eventName,
                eventData, cancellationToken);
        }
        public async Task<T> GetStateAsync<T>(string storeName, string key)
        {
            return await daprClient.GetStateAsync<T>(storeName, key);
        }
        public async Task SaveStateAsync<T>(string storeName, string key, T value)
        {
            await daprClient.SaveStateAsync(storeName, key, value);
        }
        protected DaprClient daprClient { get; }
        public HttpContext httpContext { get; }
    }
}
