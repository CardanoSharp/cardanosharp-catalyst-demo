using CardanoSharp.CatalystDemo.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CardanoSharp.CatalystDemo.ViewModels
{
    public class WalletViewModel : BaseViewModel
    {
        private string mnemonicWords;

        public SendForm SendForm { get; set; }

        public Command GenerateWallet { get; }
        public Command RestoreWallet { get; }
        public Command RefreshUtxos { get; }
        public Command SubmitTx { get; }

        private readonly IWalletService _walletService;
        private readonly IBlockfrostService _blockfrostService;
        private readonly ITransactionService _transactionService;

        private readonly IWalletStore _walletStore;

        public WalletViewModel(IWalletService walletService, 
            IBlockfrostService blockfrostService, 
            ITransactionService transactionService,
            IWalletStore walletStore)
        {
            _walletService = walletService;
            _blockfrostService = blockfrostService;
            _transactionService = transactionService;
            _walletStore = walletStore;
        }

        private async void OnGenerateWallet()
        {
            var mnemonic = await _walletService.GenerateMnemonic(15);
            _walletStore.SetAccountKeys(mnemonic);
        }

        private async void OnRestoreWallet()
        {
            var mnemonic = await _walletService.RestoreMnemonic("");
            _walletStore.SetAccountKeys(mnemonic);
        }

        public async void OnRefreshUtxos()
        {
            var utxos = _blockfrostService.GetUtxos("");
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

            var transaction = await _transactionService.Send(request, keyPair);
        }
    }

    public class SendForm
    {
        public string SenderAddress { get; set; }
        public string RecieverAddress { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
    }
}
