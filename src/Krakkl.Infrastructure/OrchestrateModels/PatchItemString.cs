using Newtonsoft.Json;

namespace Krakkl.Infrastructure.OrchestrateModels
{
    public class PatchItemString
    {
        [JsonProperty("op")]
        public string Op { get; set; }
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
