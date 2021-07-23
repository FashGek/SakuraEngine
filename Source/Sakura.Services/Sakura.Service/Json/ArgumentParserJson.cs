using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sakura.Service
{
    internal static partial class AsyncArgumentsParser
    {
        public static JsonElement? TryGetProperty2(this JsonElement Element, string Name)
        {
            JsonElement value;
            bool bGet = Element.TryGetProperty(Name, out value);
            return bGet ? value : null;
        }

        public static async ValueTask<object> ParseJsonToParameters(MethodInfo Method, System.IO.Stream JsonStream)
        {
            // Gets the JSON and parses it
            JsonElement? JParams = null;
            try
            {
                JParams = await JsonSerializer.DeserializeAsync<JsonElement?>(JsonStream);
            }
            catch (JsonException E)
            {
                JParams = null;
                Console.WriteLine(E);
            }
            return Method.GetParameters()
                ?.Select(p => Convert.ChangeType(JParams?.TryGetProperty2(p.Name)?.ToString()??p.DefaultValue, p.ParameterType)).ToArray()
                ??null;
        }
    }
}
