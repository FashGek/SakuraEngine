using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakura.Service
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceAPIAttribute : Attribute
    {
        public ServiceAPIAttribute(string Name, ServiceDataFormat DataFormat = ServiceDataFormat.JSON)
        {
            this.Name = Name;
            this.DataFormat = DataFormat;
        }
        public readonly string Name;
        public readonly ServiceDataFormat DataFormat;
    }

    
    public enum ServiceDataFormat
    {
        JSON,
        YAML,
        UNKNOWN
    }
    [AttributeUsage(AttributeTargets.ReturnValue)]
    public class ServiceResponseAttribute : Attribute
    {
        public ServiceResponseAttribute(ServiceDataFormat DataFormat)
        {
            this.DataFormat = DataFormat;
        }
        public readonly ServiceDataFormat DataFormat;
    }
}
