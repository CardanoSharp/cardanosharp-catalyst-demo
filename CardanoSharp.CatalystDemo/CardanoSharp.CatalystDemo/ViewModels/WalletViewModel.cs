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

        public Command GenerateWallet { get; }
        public Command RestoreWallet { get; }
        public Command RefreshUtxos { get; }
        public Command SubmitTx { get; }

        private readonly IWalletService _walletService;
        private readonly IBlockfrostService _blockfrostService;

        public WalletViewModel(IWalletService walletService, IBlockfrostService blockfrostService)
        {
            _walletService = walletService;
            _blockfrostService = blockfrostService;
        }

        private async void OnGenerateWallet()
        {
            var mnemonic = _walletService.GenerateMnemonic(15);
        }

        private async void OnRestoreWallet()
        {
            var mnemonic = _walletService.RestoreMnemonic("");
        }

        public async void OnRefreshUtxos()
        {
            var utxos = _blockfrostService.GetUtxos("");
        }

        public async void OnSubmitTx()
        {

        }
    }
}
