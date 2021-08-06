using System;
using System.Collections.Generic;

namespace Sakura.Service
{
    public class InvokeFailureInfo
    {
        public string ParameterName { get; internal set; }
        public string ParameterType { get; internal set; }
        public string ArgumentType  { get; internal set; }
    }

    public class InvalidArgumentsException : Exception
    {
        public InvalidArgumentsException(object[] Arguments, System.Reflection.ParameterInfo[] ParameterInfos)
        {
            this.Arguments = Arguments;
            this.ParameterInfos = ParameterInfos;
            for (int i = 0; i < Arguments.Length; i++)
            {
                var Parameter = ParameterInfos[i];
                var Argument = Arguments[i];
                if (!Parameter.ParameterType.IsAssignableFrom(Argument.GetType()))
                {
                    FailureInfos.Add(new InvokeFailureInfo
                    {
                        ParameterName = Parameter?.Name,
                        ParameterType = Parameter?.ParameterType?.Name,
                        ArgumentType = Argument.GetType()?.Name
                    });
                }
            }
        }

        public override string ToString()
        {
            return $"Parse Error!\n" +
                   $"Failure Infos: {System.Text.Json.JsonSerializer.Serialize(FailureInfos)}\n";
        }

        public List<InvokeFailureInfo> FailureInfos { get; } = new List<InvokeFailureInfo>();
        [System.Text.Json.Serialization.JsonIgnore] public object[] Arguments { get; }
        [System.Text.Json.Serialization.JsonIgnore] public System.Reflection.ParameterInfo[] ParameterInfos { get; }
    }
}
