namespace Sakura.Service
{
    using System.Text.Json;
    public class JsonServiceNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name;
        }
        public static JsonNamingPolicy Policy { get; } = new JsonServiceNamingPolicy();
    }
}