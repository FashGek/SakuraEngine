using System;

namespace Sakura.Service
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceTopicAttribute : ServiceAPIAttribute
    {
        public ServiceTopicAttribute(string PubsubName, string Name)
            :base(Name)
        {
            this.PubsubName = PubsubName;
        }
        public string PubsubName { get; }
    }
}