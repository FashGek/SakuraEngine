namespace Sakura.VersionControl
{
    using Sakura.Service;

    public partial class VersionControlService
    {
        [ServiceAPI("Perforce/Loggin")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public bool LogginPerforce(string Server, string User, string Workspace = "")
        {
            Connection = new PerfoceConnection(Server, User, Workspace);
            return Connection.UserConnected;
        }

        [ServiceAPI("Perforce/State")]
        [return: ServiceResponse(ServiceDataFormat.JSON)]
        public PerforceState QueryPerforce()
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

        PerfoceConnection Connection = new PerfoceConnection();
    }
}
