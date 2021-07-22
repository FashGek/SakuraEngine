using Perforce.P4;

namespace Sakura.VersionControl
{
    public struct PerforceState
    {
        public string UserName { get; set; }
        public string ServerIP { get; set; }
        public bool ServerConnected { get; set; }
        public bool UserConnected { get; set; }
        public string CharacterSet { get; set; }
    }
}
