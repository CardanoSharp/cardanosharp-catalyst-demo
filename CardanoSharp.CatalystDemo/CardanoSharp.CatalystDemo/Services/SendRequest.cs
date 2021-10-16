namespace CardanoSharp.CatalystDemo.Services
{
    public class SendRequest
    {
        public string SenderAddress { get; set; }
        public string RecieverAddress { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }

    }
}
