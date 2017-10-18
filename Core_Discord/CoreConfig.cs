using Newtonsoft.Json;

namespace Core_Discord
{
    class CoreConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; } = string.Empty;

        [JsonProperty("command_prefix")]
        public string CommandPrefix { get; private set; } = "d#+";

        [JsonProperty("shards")]
        public int ShardCount { get; private set; } = 1;

        [JsonProperty("user")]
        public bool UseUserToken { get; private set; } = false;
    }
}
