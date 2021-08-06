using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakura.Service
{
    public interface IMachineContext
    {
        public string MachineName => System.Environment.MachineName;
    }
}
