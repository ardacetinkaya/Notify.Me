using System;
using Newtonsoft.Json;

namespace NotifyMe.Services
{
    public class ChatMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }


        public DateTimeOffset Date { get; set; }

    }
}