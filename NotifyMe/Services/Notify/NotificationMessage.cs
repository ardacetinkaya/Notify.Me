using System;
using Newtonsoft.Json;

namespace NotifyMe.Services
{
    public class NotificationMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }

        public DateTimeOffset Date { get; set; }

    }
}