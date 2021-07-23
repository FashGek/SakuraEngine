namespace Sakura.Service
{
    using System.IO;
    using System.Threading.Tasks;
    using System.Text.Json;

    public static class DaprCLI
    {
        public static System.IO.StreamReader ConsoleExecute(string command, string args)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                }
            };
            process.Start();
            process.StandardInput.WriteLine();
            process.StandardInput.AutoFlush = true;
            process.StandardInput.WriteLine("exit");

            StreamReader reader = process.StandardOutput;
            return reader;
        }

        public static System.IO.StreamReader DaprListJsonStream(bool Kubernetes = false)
            => ConsoleExecute("dapr", Kubernetes ? "list -k --output json" : "list --output json");

        public static async ValueTask<DaprListResult[]> DaprList(bool Kubernetes = false) 
            => await JsonSerializer.DeserializeAsync<DaprListResult[]>(
                DaprListJsonStream(Kubernetes).BaseStream
            );
    }
}
