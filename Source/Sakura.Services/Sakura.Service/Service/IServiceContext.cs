namespace Sakura.Service
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IServiceContext
    {
        public Task<T> GetStateAsync<T>(string storeName, string key);
        public Task SaveStateAsync<T>(string storeName, string key, T value);
        public Task PublishEventAsync<T>(string pubsubName, string eventName,
            T eventData, CancellationToken cancellationToken = default(CancellationToken));
        public Task PublishEventAsync(string pubsubName, string eventName,
            CancellationToken cancellationToken = default(CancellationToken));
        public Task InvokeMethodAsync<TRequest>(string appId, string methodName,
            TRequest data, CancellationToken cancellationToken = default);
        public Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(string appId, string methodName, 
            TRequest data, CancellationToken cancellationToken = default);
    }
}
