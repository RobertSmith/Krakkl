using Newtonsoft.Json;

namespace Krakkl.Persistence
{
    public class PatchItemObject
    {
        [JsonProperty("op")]
        public string Op { get; set; }
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
