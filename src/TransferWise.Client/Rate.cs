using System;
using Newtonsoft.Json;

namespace TransferWise.Client
{
    public class Rate
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("rate")]
        public decimal Value { get; set; } 
    }
}