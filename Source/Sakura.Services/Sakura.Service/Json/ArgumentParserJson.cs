using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sakura.Service
{
    internal static partial class AsyncArgumentsParser
    {
        public static object ToObject(this JsonElement element, Type type)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize(json, type);
        }
        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }
        public static T ToObject<T>(this JsonDocument document)
        {
            var json = document.RootElement.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }

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
            Console.WriteLine(JParams.ToString());
            return Method.GetParameters()
                ?.Select(p => JParams?.TryGetProperty2(p.Name)?.ToObject(p.ParameterType)??p.DefaultValue).ToArray()
                ??null;
        }
    }
}
