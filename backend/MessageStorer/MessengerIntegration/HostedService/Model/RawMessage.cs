using Newtonsoft.Json;
using System.Collections.Generic;

namespace MessengerIntegration.HostedService.Model
{
    public class RawMessage
    {
        [JsonProperty("sender_name")]
        public string SenderName { get; set; }

        [JsonProperty("timestamp_ms")]
        public long TimestampMs { get; set; }

        public string Content { get; set; }

        public List<RawAttachment> Photos { get; set; }

        public List<RawAttachment> Videos { get; set; }

        public List<RawAttachment> Gifs { get; set; }

        [JsonProperty("audio_files")]
        public List<RawAttachment> Audio { get; set; }

        public List<RawAttachment> Files { get; set; }
    }
    public class RawAttachment
    {
        public string Uri { get; set; }
    }
}
