using System;

namespace Sakura.Service
{
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