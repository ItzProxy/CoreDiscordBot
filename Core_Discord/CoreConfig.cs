using Newtonsoft.Json;

namespace Core_Discord
{
    public sealed class CoreConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; } = string.Empty;

        [JsonProperty("command_prefix")]
        public string CommandPrefix { get; private set; } = "!c";

        [JsonProperty("shards")]
        public int ShardCount { get; private set; } = 1;

        [JsonProperty("user")]
        public bool UseUserToken { get; private set; } = false;
    }
}
