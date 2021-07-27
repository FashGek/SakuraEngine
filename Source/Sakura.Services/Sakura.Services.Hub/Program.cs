namespace Sakura.Services.Hub
{
    using Sakura.Service;
    using System;
    using System.Linq;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start");
            try
            {
                string st = DaprCLI.DaprListJsonStream().ReadToEnd();
                var DaprList = DaprCLI.DaprList().Result;
                var SakuraCLIExisted = DaprList is null ? null : from Dapr in DaprList
                                       where Dapr.appId == "SakuraCLI"
                                       select Dapr;        
                var FileDialogExisted = DaprList is null ? null : from Dapr in DaprList
                                       where Dapr.appId == "SakuraFileDialog"
                                       select Dapr;
                var AssetServiceExisted = DaprList is null ? null : from Dapr in DaprList
                                       where Dapr.appId == "SakuraAsset"
                                       select Dapr;

                if (SakuraCLIExisted is null || !SakuraCLIExisted.Any())
                {
                    // Acquire CLI Service 
                    CLIService = new CloudServiceInstance(
                        "Sakura.Services.CLI.dll",
                        "SakuraCLI", 5000, 5005,
                        CloudServiceInstance.CloudServiceLanguage.ASPDotNet);
                    Console.WriteLine("CLIService Started");
                }
                else
                {
                    CLIService = new CloudServiceInstance(SakuraCLIExisted.ElementAt(0).pid);
                    Console.WriteLine("CLIService Finded");
                }

                if (SakuraCLIExisted is null || !SakuraCLIExisted.Any())
                {
                    // Acquire FileDialog Service 
                    FileDialogService = new CloudServiceInstance("Sakura.Services.FileDialog.py",
                        "SakuraFileDialog", 5010, 5015,
                        CloudServiceInstance.CloudServiceLanguage.Python);
                    Console.WriteLine("FileDialogService Started");
                }
                else
                {
                    FileDialogService = new CloudServiceInstance(FileDialogExisted.ElementAt(0).pid);
                    Console.WriteLine("FileDialogService Finded");
                }

                if (AssetServiceExisted is null || !AssetServiceExisted.Any())
                {
                    // Acquire FileDialog Service 
                    AssetService = new CloudServiceInstance("Sakura.Services.Asset.dll",
                        "SakuraAsset", 5020, 5025,
                        CloudServiceInstance.CloudServiceLanguage.ASPDotNet);
                    Console.WriteLine("AssetService Started");
                }
                else
                {
                    AssetService = new CloudServiceInstance(AssetServiceExisted.ElementAt(0).pid);
                    Console.WriteLine("AssetService Finded");
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E);
            }

            while (CLIService != null || FileDialogService != null || AssetService != null)
            {

            }
            return;
        }

        static CloudServiceInstance CLIService;
        static CloudServiceInstance FileDialogService;
        static CloudServiceInstance AssetService;
    }
}