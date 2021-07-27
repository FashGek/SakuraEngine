namespace Sakura.Services.Asset
{
    using Sakura.Service;
    using Sakura.AssetPipeline;
    using System.Linq;

    public class Program
    {
        [ServiceAPI("ListTypes")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public AssetType[] AssetTypesList() => AssetType.AllTypes.Values.ToArray();

        [ServiceAPI("ListTypeNames")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string[] AssetTypeNamesList() => AssetType.AllTypes.Keys.ToArray();

        [ServiceAPI("TypeRegister")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool AssetTypeRegister(string Name, string[] Exts = null) => AssetType.RegisterAssetType(Name, Exts);

        [ServiceAPI("TypeAttachExt")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool AssetTypeAttachExt(string Name, string Ext) => AssetType.AttachExtension(Name, Ext);

        public static void Main(string[] args) => CloudService.Run<Program>(args);
    }
}