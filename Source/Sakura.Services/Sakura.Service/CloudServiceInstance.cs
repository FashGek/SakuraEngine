using System;

namespace Sakura.Service
{
    public class CloudServiceInstance
    {
        public enum CloudServiceLanguage
        {
            ASPDotNet,
            Python,
            PHP,
            Java
        }

        public enum DeployAt
        {
            Local,
            Kubernetes
        }

        public CloudServiceInstance(int Id)
        {
            Process = System.Diagnostics.Process.GetProcessById(Id);
            //ConsoleReader = Process.StandardOutput;
        }

        public CloudServiceInstance(string ExecName, string AppId, int AppPort, int HttpPort, CloudServiceLanguage Language)
        {
            try
            {
                this.Language = Language;
                string args = " run ";
                args += $"--app-id {AppId} ";
                args += $"--app-port {AppPort} ";
                args += $"--dapr-http-port {HttpPort} ";
                switch (Language)
                {
                    case CloudServiceLanguage.ASPDotNet:
                        args += $"-- dotnet {ExecName} --urls \"http://*:{AppPort}\" ";
                        break;
                    case CloudServiceLanguage.Python:
                        args += $"-- python {ExecName}";
                        break;
                    default:
                        throw new NotImplementedException("Only Support Python & C# Services Now!");
                }
                Process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "dapr",
                        Arguments = args,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = false,
                        RedirectStandardInput = false
                    }
                };
                Process.Start();
                //ConsoleReader = Process.StandardOutput;
            }
            catch (Exception E)
            {
                Console.WriteLine(E);
            }
        }

        //System.IO.StreamReader ConsoleReader;
        System.Diagnostics.Process Process;
        CloudServiceLanguage Language;
        DeployAt Deployment = DeployAt.Local;
    }
}
