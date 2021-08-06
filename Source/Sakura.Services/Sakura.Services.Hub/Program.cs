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
                    CLIService = new ServiceApplication(
                        "Sakura.Services.CLI.dll",
                        "SakuraCLI", 5000, 5005,
                        ProgramPlatform.ASPDotNet);
                    Console.WriteLine("CLIService Started");
                }
                else
                {
                    CLIService = new ServiceApplication(SakuraCLIExisted.ElementAt(0).pid);
                    Console.WriteLine("CLIService Finded");
                }

                if (SakuraCLIExisted is null || !SakuraCLIExisted.Any())
                {
                    // Acquire FileDialog Service 
                    FileDialogService = new ServiceApplication("Sakura.Services.FileDialog.py",
                        "SakuraFileDialog", 5010, 5015,
                        ProgramPlatform.Python);
                    Console.WriteLine("FileDialogService Started");
                }
                else
                {
                    FileDialogService = new ServiceApplication(FileDialogExisted.ElementAt(0).pid);
                    Console.WriteLine("FileDialogService Finded");
                }

                if (AssetServiceExisted is null || !AssetServiceExisted.Any())
                {
                    // Acquire FileDialog Service 
                    AssetService = new ServiceApplication("Sakura.Services.Asset.dll",
                        "SakuraAsset", 5020, 5025,
                        ProgramPlatform.ASPDotNet);
                    Console.WriteLine("AssetService Started");
                }
                else
                {
                    AssetService = new ServiceApplication(AssetServiceExisted.ElementAt(0).pid);
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

        static ServiceApplication CLIService;
        static ServiceApplication FileDialogService;
        static ServiceApplication AssetService;
    }
}