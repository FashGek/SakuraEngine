namespace Sakura.VersionControl
{
    using Sakura.Service;
    using System.Linq;

    public partial class VersionControlService
    {
        [ServiceAPI("Perforce/Loggin")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool PerforceLoggin(IServiceContext Context, string Server, string User, string Workspace = "")
        {
            Connection = new PerfoceConnection(Server, User, Workspace);
            return Connection.UserConnected;
        }

        [ServiceAPI("Perforce/State")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public PerforceState PerforceQuery(IServiceContext Context)
        {
            PerforceState State = new PerforceState();
            State.ServerConnected = Connection.ServerConnected;
            if (State.ServerConnected)
            {
                State.ServerIP = Connection.Server.Address.ToString();
                if (Connection.UserConnected)
                {
                    State.UserName = Connection.User.ToString();
                    State.CharacterSet = Connection.CharacterSet.ToString();
                }
            }
            return State;
        }

        [ServiceAPI("Perforce/SyncDirectory")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public void PerforceSyncDirectory(IServiceContext Context, string Workspace, string FileName)
        {
            if (Connection.UserConnected)
            {
                var P4WSs = from ws in Connection.WorkSpaces
                            where ws.Name == Workspace
                            select ws;

                if (!P4WSs.Any()) return;
                var P4WS = P4WSs.ElementAt(0);
                //P4WS.SyncFiles();
            }
        }

        PerfoceConnection Connection = new PerfoceConnection();
    }
}
