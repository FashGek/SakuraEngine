namespace Sakura.Service
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Text.Json;
    using System.Text.Unicode;
    using System.Threading.Tasks;
    using Dapr.Client;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    internal class DaprServiceStartup<T> where T : new()
    {
        public IConfiguration Configuration { get; }
        static DaprServiceStartup()
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

        public DaprServiceStartup(IConfiguration configuration)
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
                    var TopicAttr = APIAttr as ServiceTopicAttribute;
                    if (TopicAttr is not null)
                    {
                        NamedTopicAttrs.Add(TopicAttr.Name, TopicAttr);
                    }
                }

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
            services.AddSingleton(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = ServiceJsonNamingPolicy.Policy,
                PropertyNameCaseInsensitive = false
            });
            services.AddDaprClient(builder => builder.UseJsonSerializationOptions(
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = ServiceJsonNamingPolicy.Policy,
                    PropertyNameCaseInsensitive = false
                }
            ));
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
                    var Name = kv.Key;
                    var Method = kv.Value;
                    var Mapped = endpoints.MapPost(Name, Binder);
                    if (NamedTopicAttrs.TryGetValue(Name, out var TopicAttr))
                    {
                        var Pubsubed = Mapped.WithTopic(TopicAttr.PubsubName, Name);
                    }

                    async Task Binder(HttpContext context)
                    {
                        var Client = context.RequestServices.GetRequiredService<DaprClient>();
                        object[] Arguments = null;
                        // 
                        {
                            Arguments = await AsyncArgumentsParser.ParseStreamToParameters(Method,
                                context.Request.Body, RequestParamTypes.GetValueOrDefault(kv.Key)
                            ) as object[];
                            var ServiceContext = new ServiceContext(Client, context);
                            object[] ArgumentAt0 = new object[] { ServiceContext };
                            if (Arguments is not null)
                            {
                                Arguments[0] = ServiceContext;
                            }
                            else
                            {
                                Arguments = ArgumentAt0;
                            }
                        }

                        try
                        {
                            // Invoke
                            var Result = Method.Invoke(ServiceImpl, Arguments); // Invoke
                            if (Result is not Task || Result != null)
                            {
                                Console.WriteLine("Result is {0}.", Result);
                            }
                            await JsonSerializer.SerializeAsync(context.Response.Body, Result);
                        }
                        catch (ArgumentException E)
                        {
                            context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest; // return 400
                            var ServiceException = new InvalidArgumentsException(Arguments, Method.GetParameters());
                            await JsonSerializer.SerializeAsync(context.Response.Body, ServiceException);
                            Console.WriteLine($"Return Code {context.Response.StatusCode}, Error:\n {ServiceException}.");
                        }
                    }
                }
            });
        }

        protected T ServiceImpl;
        static MethodInfo StoppingDelegate = null;
        static MethodInfo StoppedDelegate = null;
        static MethodInfo StartupDelegate = null;
        static Dictionary<string, MethodInfo> NamedRequestDelegates = new Dictionary<string, MethodInfo>();
        static Dictionary<string, ServiceTopicAttribute> NamedTopicAttrs = new Dictionary<string, ServiceTopicAttribute>();
        static Dictionary<string, ServiceDataFormat> RequestRVTypes = new Dictionary<string, ServiceDataFormat>();
        static Dictionary<string, ServiceDataFormat> RequestParamTypes = new Dictionary<string, ServiceDataFormat>();
    }
}
