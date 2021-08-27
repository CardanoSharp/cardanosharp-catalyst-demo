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

        public WalletViewModel(IWalletService walletService)
        {

        }

        private async void OnGenerateWallet()
        {

        }

        private async void OnRestoreWallet()
        {

        }

        public async void OnRefreshUtxos()
        {

        }

        public async void OnSubmitTx()
        {

        }
    }
}
