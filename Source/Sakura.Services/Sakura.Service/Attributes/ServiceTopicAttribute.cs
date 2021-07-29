using System;

namespace Sakura.Service
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceTopicAttribute : Attribute
    {
        public ServiceTopicAttribute(string pubsubName, string name)
        {
            this.Name = name;
            this.PubsubName = pubsubName;
        }
        public string PubsubName { get; }
        public string Name { get; }
    }
}