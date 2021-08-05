namespace Sakura.AssetPipeline
{
    using Spreads.LMDB;
    using Spreads.Utils;

    public class LocalManifest
    {
        public LocalManifest(string WorkspaceName, string DatabaseLocation)
        {
            this.WorkspaceName = WorkspaceName;
            this.DatabaseLocation = DatabaseLocation;

            initializeDatabase();
        }

        protected void initializeDatabase()
        {
            System.Console.WriteLine(LMDBVersionInfo.Version);
        }
            
        public readonly string WorkspaceName;
        public readonly string DatabaseLocation;
    }
}
