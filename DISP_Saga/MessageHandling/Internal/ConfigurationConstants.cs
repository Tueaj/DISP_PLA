using Newtonsoft.Json;

namespace MessageHandling.Internal
{
    internal static class ConfigurationConstants
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
        };
    
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return JsonSerializerSettings;
        }
    }
}