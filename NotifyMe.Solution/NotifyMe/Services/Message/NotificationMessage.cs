using System;
using Newtonsoft.Json;

namespace NotifyMe.Services
{
    public class NotificationMessage : BaseMessage
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }

    }
}