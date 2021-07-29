using System;

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
}
