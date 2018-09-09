using System;
using Newtonsoft.Json;

namespace NotifyMe.Services
{
    public class BaseMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

    }
}