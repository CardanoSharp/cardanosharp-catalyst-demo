using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CardanoSharp.CatalystDemo.ViewModels
{
    public class AboutViewModel : BindableObject
    { 
        public AboutViewModel()
        {
            GenerateWallet = new Command(OnGenerateWallet);
            CreateTransaction = new Command(OnCreateTransaction);
        }
        string _mnemonic = "";
        public string Mnemonic
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
        string _sendToAddress = "";
        public string SendToAddress
        {
            get => _sendToAddress;
            set
            {
                if (value == _sendToAddress)
                    return;
                _sendToAddress = value;
                OnPropertyChanged(nameof(SendToAddress));
            }
        }

        Double _sendToAmount = 0;
        public Double SendToAmount
        {
            get => _sendToAmount;
            set
            {
                if (value == _sendToAmount)
                    return;
                _sendToAmount = value;
                OnPropertyChanged(nameof(SendToAmount));
            }
        }

        string _transactionLink = "";
        public string TransactionLink
        {
            get => _transactionLink;
            set
            {
                if (value == _transactionLink)
                    return;
                _transactionLink = value;
                OnPropertyChanged(nameof(TransactionLink));
            }
        }

        public ICommand GenerateWallet { get; }
        void OnGenerateWallet()
        {
            Mnemonic = "Test test test test test";
            WalletCreated = true;
        }

        public ICommand CreateTransaction { get; }
        void OnCreateTransaction()
        {
            TransactionLink = "http://localhost:3000";
        }
    }
}