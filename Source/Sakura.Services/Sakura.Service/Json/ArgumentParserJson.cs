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
            return bGet ? value : new JsonElement();
        }

        public static async ValueTask<object> ParseJsonToParameters(MethodInfo Method, System.IO.Stream JsonStream)
        {
            // Gets the JSON and parses it
            try
            {
                var JsonParameters = await JsonSerializer.DeserializeAsync<JsonElement>(JsonStream);
                var Parameters = Method.GetParameters()
                    .Select(p => Convert.ChangeType(JsonParameters.TryGetProperty2(p.Name).ToString(), p.ParameterType))
                    .ToArray();
                return Parameters;
            }
            catch (JsonException E)
            {
                Console.WriteLine(E);
                return null;
            }
        }
    }
}
