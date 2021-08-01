using System;
using System.ComponentModel.DataAnnotations;

namespace CardanoSharp.CatalystDemo
{
    public class SendFormRequest
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public int Amount { get; set; }
    }
}
