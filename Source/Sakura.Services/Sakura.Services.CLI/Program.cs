namespace Sakura.Services.CLI
{
    using Sakura.Service;

    public class Program
    {
        [ServiceAPI("Dapr/List")][return: ServiceResponse(ServiceDataFormat.JSON)]
        public string DaprList(IServiceContext Context) => DaprCLI.DaprListJsonStream().ReadToEnd();
        
        [ServiceAPI("Service/ListAPI")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public string ServiceListAPI(IServiceContext Context, string Service)
        {
            return null;
        }

        public static void Main(string[] args) => ServiceProgram.Run<Program>(args);
    }
}