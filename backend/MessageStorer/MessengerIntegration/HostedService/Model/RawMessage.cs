using Newtonsoft.Json;

namespace MessengerIntegration.HostedService.Model
{
    public class RawMessage
    {
        [JsonProperty("sender_name")]
        public string SenderName { get; set; }

        [JsonProperty("timestamp_ms")]
        public long TimestampMs { get; set; }

        public string Content { get; set; }
    }
}
