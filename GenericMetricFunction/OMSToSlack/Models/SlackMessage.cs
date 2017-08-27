using Newtonsoft.Json;

namespace OMSToSlack.Models
{
    public class SlackMessage
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("link_names")]
        public bool LinkNames { get; set; }
    }
}