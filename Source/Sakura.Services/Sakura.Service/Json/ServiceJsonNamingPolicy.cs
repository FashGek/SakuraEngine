namespace Sakura.Service
{
    using System.Text.Json;
    public class ServiceJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name;
        }
        public static JsonNamingPolicy Policy { get; } = new ServiceJsonNamingPolicy();
    }
}