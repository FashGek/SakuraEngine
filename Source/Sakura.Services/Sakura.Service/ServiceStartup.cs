namespace Sakura.Service
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Dapr.Client;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    internal class ServiceStartup<T> where T : new()
    {
        public IConfiguration Configuration { get; }
        static ServiceStartup()
        {
            var Methods = typeof(T).GetMethods();
            if (Methods is null) return;
            foreach (var Method in Methods)
            {
                var APIAttr = Method.GetCustomAttribute<ServiceAPIAttribute>();
                if (APIAttr is null) 
                    continue;
                else
                    RequestParamTypes.Add(APIAttr.Name, APIAttr.DataFormat);

                var ParamAttr = Method.ReturnType.GetCustomAttribute<ServiceResponseAttribute>();
                if (ParamAttr != null)
                {
                    RequestRVTypes.Add(APIAttr.Name, ParamAttr.DataFormat); 
                }
                else
                {
                    RequestRVTypes.Add(APIAttr.Name, ServiceDataFormat.JSON); // Json By Default.
                }
            }
        }

        public ServiceStartup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            ServiceImpl = new T();

            var Methods = typeof(T).GetMethods();
            if (Methods is null) return;
            foreach (var Method in Methods)
            {
                var APIAttr = Method.GetCustomAttribute<ServiceAPIAttribute>();
                if (APIAttr is not null)
                {
                    NamedRequestDelegates.Add(APIAttr.Name, Method);
                }
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDaprClient();
            services.AddSingleton(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();

                foreach (var kv in NamedRequestDelegates)
                {
                    endpoints.MapPost(kv.Key, Binder);

                    async Task Binder(HttpContext context)
                    {
                        var Client = context.RequestServices.GetRequiredService<DaprClient>();
                        var Method = kv.Value;
                        var Arguments = await AsyncArgumentsParser.ParseStreamToParameters(Method,
                            context.Request.Body, RequestParamTypes.GetValueOrDefault(kv.Key)
                        ) as object[];
                        
                        var Result = Method.Invoke(ServiceImpl, Arguments); // Invoke
                        if (Result != null)
                        {
                            Console.WriteLine("Result is {0}.", Result);
                        }
                        await JsonSerializer.SerializeAsync(context.Response.Body, Result);
                    }
                }
            });
        }

        protected T ServiceImpl;
        static Dictionary<string, MethodInfo> NamedRequestDelegates = new Dictionary<string, MethodInfo>();
        static Dictionary<string, ServiceDataFormat> RequestRVTypes = new Dictionary<string, ServiceDataFormat>();
        static Dictionary<string, ServiceDataFormat> RequestParamTypes = new Dictionary<string, ServiceDataFormat>();
    }
}
