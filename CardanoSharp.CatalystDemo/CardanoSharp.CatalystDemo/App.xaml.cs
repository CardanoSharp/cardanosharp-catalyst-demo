using CardanoSharp.CatalystDemo.Services;
using CardanoSharp.CatalystDemo.Views;
using CardanoSharp.Wallet;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CardanoSharp.CatalystDemo
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            DependencyService.Register<IBlockfrostService, BlockfrostService>();
            DependencyService.Register<ITransactionService, TransactionService>();
            DependencyService.Register<Services.IWalletService, Services.WalletService>();
            DependencyService.Register<IWalletStore, WalletStore>();
            DependencyService.Register<IKeyService, KeyService>();
            DependencyService.Register<IAddressService, AddressService>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

    }
}
