using System;
using Blockfrost.Api.Extensions;
using CardanoSharp.CatalystDemo.Services;
using CardanoSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CardanoSharp.CatalystDemo
{
    internal class Bootstrapper
    {
        private App _app;
        private IServiceProvider _serviceProvider;

        public Bootstrapper(App app)
        {
            _app = app;
        }

        internal void Start()
        {
            ConfigureServices();
            var shell = ActivatorUtilities.CreateInstance<AppShell>(_serviceProvider);
            _app.MainPage = new NavigationPage(shell);
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            _ = services.AddBlockfrost("network", "apikey");
            //_ = services.AddScoped<IBlockfrostService, BlockfrostService>();
            //_ = services.AddScoped<ITransactionService, TransactionService>();
            _ = services.AddScoped<Services.IWalletService, Services.WalletService>();
            _ = services.AddScoped<IWalletStore, WalletStore>();
            _ = services.AddScoped<IKeyService, KeyService>();
            _ = services.AddScoped<IAddressService, AddressService>();

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}