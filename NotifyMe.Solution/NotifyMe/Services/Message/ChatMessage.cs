using System;
using Newtonsoft.Json;

namespace NotifyMe.Services
{
    public class ChatMessage :BaseMessage
    {


        [JsonProperty("friendlyusername")]
        public string FriendlyUsername { get; set; }

    }
}