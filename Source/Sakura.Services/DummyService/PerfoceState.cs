using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sakura.Services.Dummy
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
