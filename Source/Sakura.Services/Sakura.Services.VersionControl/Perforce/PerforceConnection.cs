using System;
using System.Collections.Generic;
using System.Linq;
using Perforce.P4;

namespace Sakura.VersionControl
{
    public class PerfoceConnection : SourceControlBase
    {
        public override string SolutionName => "Perforce";
        public override string CharacterSet => connection?.CharacterSetName;
        public override string User => connection?.UserName;
        public override bool ServerConnected => Server is not null && Server.State == ServerState.Online;
        public override bool UserConnected => connection is not null;
        public PerfoceConnection()
        {

        }

        public PerfoceConnection(string ServerIp, string UserName, string Workspace = "")
        {
            try
            {
                Server = new Server(new ServerAddress(ServerIp));
                rep = new Repository(Server);
                connection = rep.Connection;
                connection.UserName = UserName;
                connection.Client = new Client();
                connection.Client.Name = Workspace;
                connection.Connect(null);

                IList<Client> clients = rep.GetClients(new Options());
                localClients.AddRange(
                    from client in clients
                    where !string.IsNullOrEmpty(client.OwnerName) && client.OwnerName.Contains(User)
                    select client);
            }
            catch (Exception E)
            {
                connection = null;
                rep = null;
                Server = null;
                localClients.Clear();
                Console.WriteLine(E);
            }
        }

        public List<Client> WorkSpaces => localClients;

        public readonly Connection connection = null;
        public readonly Repository rep = null;
        public readonly Server Server = null;
        protected List<Client> localClients = new List<Client>();
    }
}
