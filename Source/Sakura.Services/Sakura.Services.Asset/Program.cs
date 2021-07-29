namespace Sakura.Services.Asset
{
    using Sakura.Service;
    using Sakura.AssetPipeline;
    using System.Linq;

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

        [ServiceAPI("BuildAsset")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool BuildAsset(IServiceContext Context, string Workspace, string PathInWorkspace)
            => false;

        public static void Main(string[] args) => CloudService.Run<Program>(args);
    }
}