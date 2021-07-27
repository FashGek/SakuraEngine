namespace Sakura.Services.CLI
{
    using Sakura.Service;
    using System.Linq;

    public class Program
    {
        [ServiceAPI("Dapr/List")][return: ServiceResponse(ServiceDataFormat.JSON)]
        public string DaprList(bool Kubernetes = false) => DaprCLI.DaprListJsonStream(Kubernetes).ReadToEnd();
        
        [ServiceAPI("RegisterStaticMesh")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool RegisterStaticMesh()
        {
            DaprListResult AssetServiceInstance = null;
            while (AssetServiceInstance is null)
            {
                var DaprList2 = DaprCLI.DaprList().Result;
                var AssetServiceExisted2 = DaprList2 is null ? null :
                                        from Dapr in DaprList2
                                        where Dapr.appId == "SakuraAsset"
                                        select Dapr;
                AssetServiceInstance = AssetServiceExisted2?.ElementAt(0) ?? null;
            }
            var Succ = CloudService.Invoke<bool>("SakuraAsset", "TypeRegister",
                new { Name = "StaticMesh", Exts = new string[] { ".fbx", ".obj" } }
            );
            return Succ;
        }

        [ServiceAPI("Service/ListAPI")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string ServiceListAPI(string Service)
        {
            return null;
        }

        public static void Main(string[] args) => CloudService.Run<Program>(args);
    }
}