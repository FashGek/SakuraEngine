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
    using Microsoft.Extensions.Logging;

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
                // API Hooks
                var APIAttr = Method.GetCustomAttribute<ServiceAPIAttribute>();
                if (APIAttr is not null)
                {
                    NamedRequestDelegates.Add(APIAttr.Name, Method);
                }
                // Topic Hooks
                var TopicAttr = Method.GetCustomAttribute<ServiceTopicAttribute>();

                // Lifetime Hooks
                var LifeTimeAttr = Method.GetCustomAttribute<ServiceLifetimeAttribute>();
                if (LifeTimeAttr is not null)
                {
                    switch(LifeTimeAttr.Section)
                    {
                        //case ServiceLifetimeSection.Stopping:
                        //    StoppingDelegate = Method; break;
                        //case ServiceLifetimeSection.Stopped:
                        //    StoppedDelegate = Method; break;
                        case ServiceLifetimeSection.Startup:
                            StartupDelegate = Method; break;
                    }
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

        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory, 
            Microsoft.Extensions.Hosting.IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseCloudEvents();
            //if (StartupDelegate is not null)
            //    appLifetime.ApplicationStarted.Register(() => StartupDelegate.Invoke(ServiceImpl, null));
            //if (StoppingDelegate is not null)
            //    appLifetime.ApplicationStopping.Register(() => StoppingDelegate.Invoke(ServiceImpl, null));
            //if (StoppedDelegate is not null)
            //    appLifetime.ApplicationStopped.Register(() => StoppedDelegate.Invoke(ServiceImpl, null));
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();

                foreach (var kv in NamedRequestDelegates)
                {
                    var Method = kv.Value;
                    var Mapped = endpoints.MapPost(kv.Key, Binder);
                    //var Pubsubed = Mapped.WithTopic(pubsubName, topicName);

                    async Task Binder(HttpContext context)
                    {
                        var Client = context.RequestServices.GetRequiredService<DaprClient>();
                        var Arguments = await AsyncArgumentsParser.ParseStreamToParameters(Method,
                            context.Request.Body, RequestParamTypes.GetValueOrDefault(kv.Key)
                        ) as object[];
                        var ServiceContext = new ServiceContext(Client, context);
                        object[] ArgumentAt0 = new object[] { ServiceContext };
                        if(Arguments is not null)
                        {
                            Arguments[0] = ServiceContext;
                        }
                        else
                        {
                            Arguments = ArgumentAt0;
                        }
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
        static MethodInfo StoppingDelegate = null;
        static MethodInfo StoppedDelegate = null;
        static MethodInfo StartupDelegate = null;
        static Dictionary<string, MethodInfo> NamedRequestDelegates = new Dictionary<string, MethodInfo>();
        static Dictionary<string, ServiceDataFormat> RequestRVTypes = new Dictionary<string, ServiceDataFormat>();
        static Dictionary<string, ServiceDataFormat> RequestParamTypes = new Dictionary<string, ServiceDataFormat>();
    }
}
