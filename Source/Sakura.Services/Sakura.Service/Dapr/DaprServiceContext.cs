namespace Sakura.Service
{
    using System.Threading;
    using System.Threading.Tasks;
    using Dapr.Client;
    using Microsoft.AspNetCore.Http;
    public class DaprServiceContext : IServiceContext
    {
        public DaprServiceContext(DaprClient client, HttpContext context)
        {
            this.daprClient = client;
            this.httpContext = context;
        }
        public DaprServiceContext()
        {
            daprClient = new DaprClientBuilder()
            .UseJsonSerializationOptions(
            new System.Text.Json.JsonSerializerOptions
            {
                DictionaryKeyPolicy = ServiceJsonNamingPolicy.Policy,
                PropertyNameCaseInsensitive = false
            }).Build();
        }
        public async Task PublishEventAsync(string pubsubName, string eventName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await daprClient.PublishEventAsync(pubsubName, eventName, cancellationToken);
        }
        public async Task PublishEventAsync<T>(string pubsubName, string eventName,
            T eventData, CancellationToken cancellationToken = default(CancellationToken))
        {
            await daprClient.PublishEventAsync<T>(pubsubName, eventName, eventData, cancellationToken);
        }
        public async Task<T> GetStateAsync<T>(string storeName, string key)
        {
            return await daprClient.GetStateAsync<T>(storeName, key);
        }
        public async Task SaveStateAsync<T>(string storeName, string key, T value)
        {
            await daprClient.SaveStateAsync(storeName, key, value);
        }
        public async Task InvokeAsync<TRequest>(string appId, string methodName,
            TRequest data, CancellationToken cancellationToken = default)
        {
            try
            {
                await daprClient.InvokeMethodAsync<TRequest>(appId, methodName, data, cancellationToken);
            }
            catch (InvocationException E)
            {
                if (E.Response is null)
                {
                    System.Console.WriteLine(E);
                }
                else
                {
                    string Content = await E.Response.Content.ReadAsStringAsync();
                    System.Console.WriteLine($"Invoke {E.AppId}::{E.MethodName} Error.\n" +
                        $"Status Code: {E.Response.StatusCode}\n" +
                        $"Content: {Content}");
                }
            }
        }
        public async Task<TResponse> InvokeAsync<TRequest, TResponse>(string appId, string methodName,
            TRequest data, CancellationToken cancellationToken = default)
        {
            try
            {
                return await daprClient.InvokeMethodAsync<TRequest, TResponse>(appId, methodName, data, cancellationToken);
            }
            catch (InvocationException E)
            {
                if (E.Response is null)
                {
                    System.Console.WriteLine(E);
                }
                else
                {
                    string Content = await E.Response.Content.ReadAsStringAsync();
                    System.Console.WriteLine($"Invoke {E.AppId}::{E.MethodName} Error.\n" +
                        $"Status Code: {E.Response.StatusCode}\n" +
                        $"Content: {Content}");
                }
                return default(TResponse);
            }
        }

        protected DaprClient daprClient { get; } = null;
        private HttpContext httpContext { get; } = null;
    }
}