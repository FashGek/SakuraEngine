namespace Sakura.Services.Dummy
{
    using Sakura.Service;
    public class Program
    {
        [ServiceAPI("Dummy/SourceControlQuery")][return: ServiceResponse(ServiceDataFormat.JSON)]
        public PerforceState QueryPerforce() => CloudService.Invoke<PerforceState>("SakuraVersionControl", "Perforce/State", null);
        public static void Main(string[] args) => new CloudService().Run<Program>(args);
    }
}