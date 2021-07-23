namespace Sakura.Services.CLI
{
    using Sakura.Service;
    public class Program
    {
        [ServiceAPI("Dapr/List")][return: ServiceResponse(ServiceDataFormat.JSON)]
        public string DaprList(bool Kubernetes = false) => DaprCLI.DaprListJsonStream(Kubernetes).ReadToEnd();
        
        [ServiceAPI("Service/Launch")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public void LaunchService(string ExecName, string AppName)
        {

        }

        [ServiceAPI("Service/ListAPI")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string ServiceListAPI(string Service)
        {
            return null;
        }

        public static void Main(string[] args) => CloudService.Run<Program>(args);
    }
}