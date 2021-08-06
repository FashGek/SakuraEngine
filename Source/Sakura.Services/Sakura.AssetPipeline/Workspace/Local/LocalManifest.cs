namespace Sakura.AssetPipeline
{
    using System;
    using System.Text;
    using LightningDB;

    public class LocalManifest : IDisposable
    {
        public LocalManifest(string LocalPath, string DatabaseLocation)
        {
            this.LocalPath = LocalPath;
            this.DatabaseLocation = DatabaseLocation;

            env = new LightningEnvironment("Library");
            {
                env.MaxDatabases = 2;
                env.Open();

                using (var tx = env.BeginTransaction())
                using (var db = tx.OpenDatabase("AssetData", new DatabaseConfiguration { Flags = DatabaseOpenFlags.Create }))
                {
                    tx.Put(db, Encoding.UTF8.GetBytes("PATH"), Encoding.UTF8.GetBytes("MetaData"));
                    tx.Commit();
                }
            }
        }

        public string QueryMeta(string Path)
        {
            using (var tx = env.BeginTransaction(beginFlags: TransactionBeginFlags.ReadOnly))
            using (var db = tx.OpenDatabase("AssetData"))
            {
                var (r, k, v) = tx.Get(db, Encoding.UTF8.GetBytes("PATH"));
                if (r == MDBResultCode.Success)
                {
                    return Encoding.UTF8.GetString(v.AsSpan());
                }
            }
            return null;
        }

        public void Dispose()
        {
            env.Dispose();
        }

        public readonly LightningEnvironment env;
        public string LocalPath { get; }
        public string DatabaseLocation { get; }
    }
}
