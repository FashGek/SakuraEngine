using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sakura.Service
{
    internal static partial class AsyncArgumentsParser
    {
        public static async ValueTask<object> ParseStreamToParameters(MethodInfo Method, System.IO.Stream Stream, ServiceDataFormat DataFormat)
        {
            if (DataFormat != ServiceDataFormat.JSON)
            {
                throw new NotImplementedException($"Service Data {DataFormat} Not Supported!");
            }

            var Params = Method.GetParameters();
            if (Params != null)
            {
                Console.WriteLine("{0} Params Passed.", Params.Length);
            }
            var Arguments = Params is null ? null :
                Params.Length <= 1 ? null
                : await ParseJsonToParameters(Method, Stream) as object[];
            if (Arguments != null)
            {
                Console.WriteLine("{0} Arguments Passed.", Arguments.Length);
            }

            return Arguments;
        }
    }
}
