namespace Sakura.Service
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IServiceContext : IMachineContext
    {
        public Task<T> GetStateAsync<T>(string storeName, string key);
        public Task SaveStateAsync<T>(string storeName, string key, T value);

        public Task PublishEventAsync<T>(string pubsubName, string eventName,
            T eventData, CancellationToken cancellationToken = default(CancellationToken));
        public Task PublishEventAsync(string pubsubName, string eventName,
            CancellationToken cancellationToken = default(CancellationToken));

        public Task InvokeAsync<TRequest>(string appId, string methodName,
            TRequest data, CancellationToken cancellationToken = default);
        public Task<TResponse> InvokeAsync<TRequest, TResponse>(string appId, string methodName, 
            TRequest data, CancellationToken cancellationToken = default);
        public TResponse Invoke<TRequest, TResponse>(string Application, string Scope, TRequest Parameters) 
            => InvokeAsync<TRequest, TResponse>(Application, Scope, Parameters).Result;
        public void Invoke<TRequest>(string Application, string Scope, TRequest Parameters)
            => InvokeAsync<TRequest>(Application, Scope, Parameters).Wait();

        public string DefaultStateStoreName => "statestore";
        public string DefaultPubsubBusName => "pubsub";
    }
}
