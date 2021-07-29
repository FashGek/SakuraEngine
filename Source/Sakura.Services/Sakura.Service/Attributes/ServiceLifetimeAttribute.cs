using System;

namespace Sakura.Service
{
    public enum ServiceLifetimeSection
    {
        //Stopped,
        //Stopping,
        Startup
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceLifetimeAttribute : Attribute
    {
        public ServiceLifetimeAttribute(ServiceLifetimeSection Section)
        {
            this.Section = Section;
        }
        public readonly ServiceLifetimeSection Section;
    }
}