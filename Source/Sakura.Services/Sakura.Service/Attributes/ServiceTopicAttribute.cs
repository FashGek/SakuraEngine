using System;

namespace Sakura.Service
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceTopicAttribute : ServiceAPIAttribute
    {
        public ServiceTopicAttribute(string PubsubName, string TopicName)
            :base(TopicName)
        {
            this.PubsubName = PubsubName;
        }
        public string PubsubName { get; }
    }
}