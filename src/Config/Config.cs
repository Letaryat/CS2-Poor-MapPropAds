using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;


namespace CS2_Poor_MapPropAds.Config
{
    public class PluginConfig : BasePluginConfig
    {
        [JsonPropertyName("Admin Flag")]
        public string AdminFlag { get; set; } = "@css/root";

        [JsonPropertyName("Vip Flag")]
        public string VipFlag { get; set; } = "@vip/noadv";

        [JsonPropertyName("Props Path")]
        public string[] Props { get; set; } = [];

        [JsonPropertyName("Enable commands")]
        public bool EnableCMD { get; set; } = true;

        [JsonPropertyName("Debug Mode")]
        public bool Debug { get; set; } = true;

    }
}
