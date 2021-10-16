using CardanoSharp.CatalystDemo.Services;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using System;
using Xamarin.Forms;
using System.Linq;
using Blockfrost.Api.Services;
using Blockfrost.Extensions.CardanoSharp;
using Blockfrost.Api.Models.Extensions;
using Blockfrost.Api;
using Microsoft.Extensions.Logging;

namespace CardanoSharp.CatalystDemo.ViewModels
{
    public class WalletViewModel : BindableObject
    {
        public WalletViewModel(
            ILogger<WalletViewModel> logger,
            IWalletService walletService,
            IWalletStore walletStore,
            ICardanoService cardanoService,
            ITransactionBuilderService txBuilder)
        {
            _logger = logger;
            _walletService = walletService;
            _walletStore = walletStore;
            _cardano = cardanoService;
            _txBuilder = txBuilder;
            
            SubmitTx = new Command(OnSubmitTx);
            RestoreWallet = new Command(OnRestoreWallet);
            GetCurrentBalance = new Command(OnGetCurrentBalance);
            GenerateWallet = new Command(OnGenerateWallet);
            RestoreForm = new RestoreForm();
            SendForm = new SendForm();
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

        //This is the last transaction id
        string _transactionId;
        public string TransactionId
        {
            get => _transactionId;
            set
            {
                if (value == _transactionId)
                    return;
                _transactionId = value.Replace("\"", ""); ;
                OnPropertyChanged(nameof(TransactionId));
            }
        }
        
        //This is the current balance
        string _currentBalance;
        public string CurrentBalance
        {
            get => _currentBalance;
            set
            {
                if (value == _currentBalance)
                    return;
                _currentBalance = value;
                OnPropertyChanged(nameof(CurrentBalance));
            }
        }

        //This is the form object we use to send ADA with Metadata message
        SendForm _sendForm;
        public SendForm SendForm
        {
            get => _sendForm;
            set
            {
                if (value == _sendForm)
                    return;
                _sendForm = value;
                OnPropertyChanged(nameof(SendForm));
            }
        }
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
        public Command GetCurrentBalance { get; }
        public Command SubmitTx { get; }

        private readonly ILogger<WalletViewModel> _logger;
        private readonly IWalletService _walletService;
        private readonly IWalletStore _walletStore;
        private readonly ITransactionBuilderService _txBuilder;
        private readonly ICardanoService _cardano;


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
            OnGetCurrentBalance();
        }

        public async void OnGetCurrentBalance()
        {
            //var utxos = await _blockfrostService.GetUtxos(Address.ToString());
            try
            {
                // F01: _addressService.ThrowsOnError = false; // would return null
                // F02: provide docs in package
                CurrentBalance = $"0.000000 ADA";
                
                var utxos = await _cardano.Addresses.GetUtxosAsync(Address.ToString());
                if (utxos != null && utxos.Any())
                {
                    var balance = utxos.SumAmounts("lovelace");
                    balance = balance / 1000000;
                    CurrentBalance = $"{balance} ADA";
                }
            }
            catch (ApiException<NotFoundResponse>)
            {
                DependencyService.Get<IToast>().ShortAlert("A fresh wallet appeared!");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while querying UTxO for {0}", Address.ToString());
            }
        }

        public async void OnSubmitTx()
        {
            var keyPair = await _walletService.GetKeyPair(_walletStore.AccountKeys, 0);

            var request = new SendRequest()
            {
                Amount = SendForm.Amount,
                RecieverAddress = SendForm.RecieverAddress,
                SenderAddress = Address.ToString(),
                Message = SendForm.Message
            };

            var transaction = await _txBuilder.BuildAsync(request, keyPair);
            
            try
            {
                var txId = await _cardano.Transactions.PostTxSubmitAsync(transaction);
                TransactionId = txId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    public class SendForm : BindableObject
    {
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
                _message = value;
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
