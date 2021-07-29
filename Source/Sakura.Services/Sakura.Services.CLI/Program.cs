namespace Sakura.Services.CLI
{
    using Sakura.Service;

    public class Program
    {
        [ServiceAPI("Dapr/List")][return: ServiceResponse(ServiceDataFormat.JSON)]
        public string DaprList(IServiceContext Context) => DaprCLI.DaprListJsonStream().ReadToEnd();
        
        [ServiceLifetime(ServiceLifetimeSection.Startup)]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool RegisterStaticMesh()
        {
            CloudService.WaitUntilServiceStarted("SakuraAsset");
            var Succ = CloudService.Invoke<bool>("SakuraAsset", "TypeRegister",
                new { Name = "StaticMesh", Exts = new string[] { ".fbx", ".obj" } }
            );
            return Succ;
        }

        public bool UnregisterStaticMesh()
        {
            CloudService.WaitUntilServiceStarted("SakuraAsset");
            var Succ = CloudService.Invoke<bool>("SakuraAsset", "TypeDettachExt",
                new { Name = "StaticMesh", Ext = ".fbx" }
            ) && CloudService.Invoke<bool>("SakuraAsset", "TypeDettachExt",
                new { Name = "StaticMesh", Ext = ".obj" }
            );
            return Succ;
        }

        [ServiceAPI("Service/ListAPI")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string ServiceListAPI(IServiceContext Context, string Service)
        {
            return null;
        }

        public static void Main(string[] args) => CloudService.Run<Program>(args);
    }
}