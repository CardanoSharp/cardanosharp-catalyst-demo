﻿using CardanoSharp.CatalystDemo.Models;
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

            SubmitTx = new Command(OnSubmitTx);
            RestoreWallet = new Command(OnRestoreWallet);
            RefreshUtxos = new Command(OnRefreshUtxos);
            GenerateWallet = new Command(OnGenerateWallet);
            RestoreForm = new RestoreForm();
        }

        bool _walletCreated = false;
        public bool WalletCreated
        {
            get => _walletCreated;
            set
            {
                if (value == _walletCreated)
                    return;
                _walletCreated = value;
                OnPropertyChanged(nameof(WalletCreated));
            }
        }

        //This holds the Address we use to Send and Receive ADA
        Address _address;
        public Address Address
        {
            get => _address;
            set
            {
                if (value == _address)
                    return;
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        //This holds our Mnemonic for display
        //note: never do this in a live app
        Mnemonic _mnemonic;
        public Mnemonic Mnemonic
        {
            get => _mnemonic;
            set
            {
                if (value == _mnemonic)
                    return;
                _mnemonic = value;
                OnPropertyChanged(nameof(Mnemonic));
            }
        }
        //This is a list of UTXOs that below to our Address
        List<Utxo> _utxos;
        public List<Utxo> Utxos
        {
            get => _utxos;
            set
            {
                if (value == _utxos)
                    return;
                _utxos = value;
                OnPropertyChanged(nameof(Utxos));
            }
        }
        //This is the last transaction id
        public string TransactionId { get; set; }

        //This is the form object we use to send ADA with Metadata message
        public SendForm SendForm { get; set; }
        //This is the form object we use to restore an existing wallet
        
        RestoreForm _restoreForm;
        public RestoreForm RestoreForm
        {
            get => _restoreForm;
            set
            {
                if (value == _restoreForm)
                    return;
                _restoreForm = value;
                OnPropertyChanged(nameof(RestoreForm));
            }
        }


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
            WalletCreated = true;
            SetupAccountNode();
        }

        public async void OnRestoreWallet()
        {
            Mnemonic = await _walletService.RestoreMnemonic(RestoreForm.Words);
            WalletCreated = true;
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
        string _senderAddress = "";
        public string SenderAddress
        {
            get => _senderAddress;
            set
            {
                if (value == _senderAddress)
                    return;
                _senderAddress = value;
                OnPropertyChanged(nameof(SenderAddress));
            }
        }

        string _recieverAddress = "";
        public string RecieverAddress
        {
            get => _recieverAddress;
            set
            {
                if (value == _recieverAddress)
                    return;
                _recieverAddress = value;
                OnPropertyChanged(nameof(RecieverAddress));
            }
        }

        decimal _amount = 0;
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (value == _amount)
                    return;
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        string _message = "";
        public string Message
        {
            get => _message;
            set
            {
                if (value == _message)
                    return;
                _recieverAddress = value;
                OnPropertyChanged(nameof(Message));
            }
        }

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
