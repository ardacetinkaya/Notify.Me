using System;
using Newtonsoft.Json;

namespace NotifyMe.Services
{
    public class ChatMessage :BaseMessage
    {
        [JsonProperty("username")]
        public string Username { get; set; }


    }
}