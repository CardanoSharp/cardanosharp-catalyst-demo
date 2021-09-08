using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.CatalystDemo.Models
{
    public class Utxo
    {
        [JsonProperty("tx_hash")]
        public string TxHash { get; set; }
        [JsonProperty("tx_index")]
        public int TxId { get; set; }
        public List<Asset> Amount { get; set; }
        public string Block { get; set; }
    }

    public class Asset
    {
        public long Quantity { get; set; }
        public string Unit { get; set; }
    }
}
