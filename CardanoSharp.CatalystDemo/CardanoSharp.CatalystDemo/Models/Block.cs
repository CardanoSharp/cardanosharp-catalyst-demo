using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.CatalystDemo.Models
{
    public class Block
    {
        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("slot")]
        public int Slot { get; set; }

        [JsonProperty("epoch")]
        public int Epoch { get; set; }

        [JsonProperty("epoch_slot")]
        public int EpochSlot { get; set; }

        [JsonProperty("slot_leader")]
        public string SlotLeader { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("tx_count")]
        public int TxCount { get; set; }

        [JsonProperty("output")]
        public string Output { get; set; }

        [JsonProperty("fees")]
        public string Fees { get; set; }

        [JsonProperty("block_vrf")]
        public string BlockVrf { get; set; }

        [JsonProperty("previous_block")]
        public string PreviousBloc { get; set; }

        [JsonProperty("next_block")]
        public string NextBlock { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

    }
}
