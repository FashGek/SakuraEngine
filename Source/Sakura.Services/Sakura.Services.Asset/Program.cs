namespace Sakura.Services.Asset
{
    using Sakura.Service;
    using System.Linq;
    using Sakura.AssetPipeline;
    using System.Threading.Tasks;

    public class Program
    {
        [ServiceAPI("ListTypes")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public AssetType[] AssetTypesList(IServiceContext Context) 
            => AssetType.AllTypes.Values.ToArray();

        [ServiceAPI("ListTypeNames")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string[] AssetTypeNamesList(IServiceContext Context) 
            => AssetType.AllTypes.Keys.ToArray();

        [ServiceAPI("TypeRegister")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool AssetTypeRegister(IServiceContext Context, string Name, string[] Exts = null) 
            => AssetType.RegisterAssetType(Name, Exts);

        [ServiceAPI("TypeAttachExt")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool AssetTypeAttachExt(IServiceContext Context, string Name, string Ext) 
            => AssetType.AttachExtension(Name, Ext);

        [ServiceAPI("TypeDettachExt")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool AssetTypeDettachExt(IServiceContext Context, string Name, string Ext) 
            => AssetType.DettachExtension(Name, Ext);

        [ServiceAPI("BindWorkspace")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool BindWorkspace(IServiceContext Context, string Workspace, string LocalPath)
            => false;

        public class Ports
        {
            string DAPR_HTTP_PORT { get; }
            string DAPR_GRPC_PORT { get; }
        }
        
        [ServiceAPI("ErrorCall")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public Ports BindWorkspace2(IServiceContext Context) => Context.Invoke<object, Ports>("nodeapp", "ports", null);

        [ServiceAPI("BuildAsset")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public void BuildAsset(IServiceContext Context, string Workspace, string PathInWorkspace)
            => Context.PublishEventAsync("pubsub", "BuildImpl", 
                new { Workspace = Workspace }
            ).Wait();
        
        [ServiceTopic("pubsub", "BuildImpl")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public void BuildAssetDummy(IServiceContext Context, string Workspace)
            => System.Console.WriteLine("Build Impl!" + Workspace);

        public static void Main(string[] args) => ServiceProgram.Run<Program>(args);
    }
}