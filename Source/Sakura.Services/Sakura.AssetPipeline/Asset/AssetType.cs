namespace Sakura.AssetPipeline
{
    using System.Collections.Generic;

    public class AssetType
    {
        static AssetType()
        {
            // Add Some Internal Asset Types.
            RegisterAssetType("json", new string[] { "json"} );
        }

        protected AssetType(string TypeName, IEnumerable<string> Exts)
        {
            if(AllTypes.ContainsKey(TypeName))
            {
                throw new System.NotSupportedException("AssetType Already Registered!");
            }
            this.TypeName = TypeName;
            if(Exts is not null)
            {
                this.ValidExtensionNames.AddRange(Exts);
            }
        }

        public static bool AttachExtension(string Name, string Extension)
        {
            if (AllTypes.TryGetValue(Name, out var Value))
            {
                System.Console.WriteLine("+" + Name);
                Value.ValidExtensionNames.Add(Extension);
                return true;
            }
            System.Console.WriteLine(Name);
            return false;
        }

        public static bool RegisterAssetType(string Name, IEnumerable<string> Exts)
        {
            AssetType NewType = new AssetType(Name, Exts);
            return AllTypes.TryAdd(Name, NewType);
        }

        public string TypeName { get; }
        public List<string> ValidExtensionNames { get; } = new List<string>();
        public static Dictionary<string, AssetType> AllTypes { get; } = new Dictionary<string, AssetType>();
    }
}
