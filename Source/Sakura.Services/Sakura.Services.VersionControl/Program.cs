namespace Sakura.VersionControl
{
    using Sakura.Service;
    using System.Threading.Tasks;

    public class SourceControlBase 
    {
        public virtual string SolutionName => "None";
        public virtual string CharacterSet => "None";
        public virtual string User => "None";
        public virtual bool ServerConnected => false;
        public virtual bool UserConnected => false;
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            VersionControlService = CloudService.Run<VersionControlService>(args);
        }

        static CloudService VersionControlService;
    }
}
