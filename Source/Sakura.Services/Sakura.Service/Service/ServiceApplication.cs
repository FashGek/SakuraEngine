using System;

namespace Sakura.Service
{
    public class ServiceApplication
    {
        public ServiceApplication(int Id)
        {
            Process = System.Diagnostics.Process.GetProcessById(Id);
        }

        public ServiceApplication(string ExecName, string AppId, int AppPort, int HttpPort, ProgramPlatform Language)
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
                    case ProgramPlatform.ASPDotNet:
                        args += $"-- dotnet {ExecName} --urls \"http://*:{AppPort}\" ";
                        break;
                    case ProgramPlatform.Python:
                        args += $"-- python3 {ExecName} 0.0.0.0 {AppPort}";
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
                        UseShellExecute = true,
                        CreateNoWindow = true,
                        RedirectStandardOutput = false,
                        RedirectStandardInput = false
                    }
                };
                Process.Start();
            }
            catch (Exception E)
            {
                Console.WriteLine(E);
            }
        }

        System.Diagnostics.Process Process;
        ProgramPlatform Language;
    }
}
