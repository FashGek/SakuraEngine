namespace Sakura.Services.Asset
{
    using Sakura.Service;
    using System.Linq;
    using Sakura.AssetPipeline;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class Program
    {
        #region AssetTypes
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
        #endregion AssetTypes

        #region Workspace
        [ServiceAPI("BindWorkspace")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool BindWorkspace(IServiceContext Context, string Workspace, string LocalPath)
        {
            if (System.IO.Directory.Exists(LocalPath))
            {
                Context.Invoke("SakuraCLI", "Environment/SetCustom", new { Name = Workspace, Value = LocalPath, Volatile = true });
                return true;
            }
            throw new System.ArgumentException($"Argument Exception: LocalPath {LocalPath} does not exist!", LocalPath);
        }
        [ServiceAPI("FindWorkspace")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string FindWorkspace(IServiceContext Context, string Workspace)
        {
            string LocalPath = Context.Invoke<object, string>("SakuraCLI", "Environment/GetCustom", new { Name = Workspace });
            return LocalPath;
        }
        #endregion Workspace

        #region FSWatcher
        [ServiceAPI("StartWatcher")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool StartWatcher(IServiceContext Context, string Workspace, string LocalPath, string DatabaseLocation) 
            => WSWatchers.TryAdd(new LocalWorkspaceInstance() {
                Workspace = Workspace,
                LocalPath = LocalPath,
                DatabaseLocation = DatabaseLocation
            }, new WorkspaceWatcher(Workspace, LocalPath, DatabaseLocation));
        Dictionary<LocalWorkspaceInstance, WorkspaceWatcher> WSWatchers { get; } = new Dictionary<LocalWorkspaceInstance, WorkspaceWatcher>();
        #endregion FSWatcher

        #region BuildPipeline
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
        #endregion BuildPipeline

        public static void Main(string[] args) => ServiceProgram.Run<Program>(args);
    }
}