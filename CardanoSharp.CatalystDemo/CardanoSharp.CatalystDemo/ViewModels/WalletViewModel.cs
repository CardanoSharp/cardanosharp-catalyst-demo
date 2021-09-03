using CardanoSharp.CatalystDemo.Models;
using CardanoSharp.CatalystDemo.Services;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CardanoSharp.CatalystDemo.ViewModels
{
    public class WalletViewModel : BindableObject
    {

        public WalletViewModel()
        {
            _walletService = DependencyService.Get<IWalletService>();
            _blockfrostService = DependencyService.Get<IBlockfrostService>();
            _transactionService = DependencyService.Get<ITransactionService>();
            _walletStore = DependencyService.Get<IWalletStore>();

            GenerateWallet = new Command(OnGenerateWallet) ;
        }

        //This holds the Address we use to Send and Receive ADA
        public Address Address { get; set; }
        //This holds our Mnemonic for display
        //note: never do this in a live app
        public Mnemonic Mnemonic { get; set; }
        //This is a list of UTXOs that below to our Address
        public List<Utxo> Utxos { get; set; }
        //This is the last transaction id
        public string TransactionId { get; set; }

        //This is the form object we use to send ADA with Metadata message
        public SendForm SendForm { get; set; }
        //This is the form object we use to restore an existing wallet
        public RestoreForm RestoreForm { get; set; }


        public Command GenerateWallet { get; }
        public Command RestoreWallet { get; }
        public Command RefreshUtxos { get; }
        public Command SubmitTx { get; }

        private readonly IWalletService _walletService;
        private readonly IBlockfrostService _blockfrostService;
        private readonly ITransactionService _transactionService;
        private readonly IWalletStore _walletStore;

        public async void OnGenerateWallet()
        {
            Mnemonic = await _walletService.GenerateMnemonic(15);
            SetupAccountNode();
        }

        public async void OnRestoreWallet()
        {
            Mnemonic = await _walletService.RestoreMnemonic(RestoreForm.Words);
            SetupAccountNode();
        }

        private async void SetupAccountNode()
        {
            _walletStore.SetAccountKeys(Mnemonic);
            Address = await _walletService.GetAddress(_walletStore.AccountKeys, 0);
        }

        public async void OnRefreshUtxos()
        {
            Utxos = await _blockfrostService.GetUtxos(Address.ToString());
        }

        public async void OnSubmitTx()
        {
            var keyPair = await _walletService.GetKeyPair(_walletStore.AccountKeys, 0);

            var request = new SendRequest()
            {
                Amount = SendForm.Amount,
                RecieverAddress = SendForm.RecieverAddress,
                SenderAddress = SendForm.SenderAddress,
                Message = SendForm.Message
            };

            var response = await _transactionService.Send(request, keyPair);
            TransactionId = response.TransactionHash;
        }
    }

    public class SendForm : BindableObject
    {
        public string SenderAddress { get; set; }
        public string RecieverAddress { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
    }

    public class RestoreForm : BindableObject
    { 
        //public string Words { get; set; }
        string _words = "";
        public string Words
        {
            get => _words;
            set
            {
                if (value == _words)
                    return;
                _words = value;
                OnPropertyChanged(nameof(Words));
            }
        }
    }
}
