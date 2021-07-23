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
                var SakuraCLIExisted = from Dapr in DaprList
                                       where Dapr.appId == "SakuraCLI"
                                       select Dapr;        
                var FileDialogExisted = from Dapr in DaprList
                                       where Dapr.appId == "SakuraFileDialog"
                                        select Dapr;

                if (!SakuraCLIExisted.Any())
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

                if (!SakuraCLIExisted.Any())
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

                DaprCLI.ConsoleExecute("dapr", "list");
            }
            catch (Exception E)
            {
                Console.WriteLine(E);
            }

            while (CLIService != null || FileDialogService != null)
            {

            }
            return;
        }

        static CloudServiceInstance CLIService;
        static CloudServiceInstance FileDialogService;
    }
}