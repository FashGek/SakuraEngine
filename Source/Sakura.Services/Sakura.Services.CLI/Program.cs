namespace Sakura.Services.CLI
{
    using Sakura.Service;
    using System.Collections.Generic;

    public class Program
    {
        [ServiceAPI("Dapr/List")][return: ServiceResponse(ServiceDataFormat.JSON)]
        public string DaprList(IServiceContext Context) => DaprCLI.DaprListJsonStream().ReadToEnd();
        
        [ServiceAPI("Service/ListAPI")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string ServiceListAPI(IServiceContext Context, string Service)
        {
            return null;
        }

        [ServiceAPI("Environment/EngineRoot")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string GetEngineRoot(IServiceContext Context) => EngineRootPath;

        [ServiceAPI("Environment/SetCustom")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public void SetCustomEnvironmentVar(IServiceContext Context, string Name, string Value, bool Volatile = false)
        {
            string StateStoreName = Context.DefaultStateStoreName;
            if (Volatile)
            {
                var res = Context.GetStateAsync<string>(StateStoreName, Name).Result;
                if (res is not null)
                {
                    throw new System.ArgumentException($"Argument Exception: Env Var {Name} must not be Volatile!", Name);
                }
                VolatileEnvVars[Name] = Value;
            }
            else
            {
                var res = VolatileEnvVars.GetValueOrDefault(Name);
                if (res is not default(string))
                {
                    throw new System.ArgumentException($"Argument Exception: Env Var {Name} must be Volatile!", Name);
                }
                Context.SaveStateAsync(StateStoreName, Name, Value).Wait();
            }
        }
        [ServiceAPI("Environment/GetCustom")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string GetCustomEnvironmentVar(IServiceContext Context, string Name, bool Volatile = false)
        {
            var res = VolatileEnvVars.GetValueOrDefault(Name);
            if (res is default(string))
            {
                res = Context.GetStateAsync<string>(Context.DefaultStateStoreName, Name).Result;
            }
            return res;
        }
        protected static Dictionary<string, string> VolatileEnvVars = new Dictionary<string, string>();


        public static void Main(string[] args) => ServiceProgram.Run<Program>(args);

        static string ServicesPath => System.IO.Path.GetFullPath(System.Environment.CurrentDirectory);
        static string EngineBinariesPath => System.IO.Directory.GetParent(ServicesPath)?.FullName;
        static string EngineRootPath => System.IO.Directory.GetParent(EngineBinariesPath)?.FullName;
    }
}