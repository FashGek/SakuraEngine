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
            => ConsoleExecute("dapr", Kubernetes ? "list -k -o json" : "list -o json");

        public static async ValueTask<ServiceInstance[]> DaprList(bool Kubernetes = false)
        {
            ServiceInstance[] results = null;
            try
            {
                results = await JsonSerializer.DeserializeAsync<ServiceInstance[]>(
                    DaprListJsonStream(Kubernetes).BaseStream
                );
            }
            catch (System.Exception E)
            {
                results = null;
            }
            return results;
        }
             
    }
}
