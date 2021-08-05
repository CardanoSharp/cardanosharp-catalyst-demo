using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using System;
using System.ComponentModel.DataAnnotations;

namespace CardanoSharp.CatalystDemo
{
    public class SendFormRequest
    {
        [Required]
        public string FromKeys { get; set; }
        [Required]
        public string ToAddress { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }

    public class RestoreWalletRequest
    {
        [Required]
        public string Words { get; set; }
    }
}
