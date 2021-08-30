using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.CatalystDemo.Models
{
    public class FeeParameters
    {
        [JsonProperty("min_fee_a")]
        public uint MinFeeA { get; set; }

        [JsonProperty("min_fee_b")]
        public uint MinFeeB { get; set; }
    }
}
