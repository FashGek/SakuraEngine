using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakura.Service
{
    public class DaprListResult
    {
        public string appId { get; set; }
        public int httpPort { get; set; }
        public int grpcPort { get; set; }
        public int appPort { get; set; }
        public bool metricsEnabled { get; set; }
        public string command { get; set; }
        public string age { get; set; }
        public string created { get; set; }
        public int pid { get; set; }
    }
}
