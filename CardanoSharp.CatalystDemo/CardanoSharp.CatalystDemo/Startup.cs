using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Blockfrost.Api.Extensions;
using CardanoSharp.CatalystDemo.Services;
using CardanoSharp.CatalystDemo.ViewModels;
using CardanoSharp.Wallet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;

namespace CardanoSharp.CatalystDemo
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static void Init()
        {
            var a = Assembly.GetExecutingAssembly();
            using (var stream = a.GetManifestResourceStream("CardanoSharp.CatalystDemo.appsettings.json"))
            {
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(c =>
                    {
                        // Tell the host configuration where to file the file (this is required for Xamarin apps)
                        _ = c.AddCommandLine(new string[] { $"ContentRoot={FileSystem.AppDataDirectory}" });

                        //read in the configuration file!

                        _ = c.AddJsonStream(stream);//.AddUserSecrets<App>();
                    })
                    .ConfigureServices((c, x) =>
                    {
                        // Configure our local services and access the host configuration
                        ConfigureServices(c, x);
                    })
                    .ConfigureLogging(l => l.AddConsole());

                var host = builder.Build();
                //Save our service provider so we can use it later.
                ServiceProvider = host.Services;
            }
        }

        static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            services.AddBlockfrost(ctx.Configuration["Network"], ctx.Configuration["ApiKey"]);

            // this ambiguity between IWalletService and Services.IWalletService is the main reason
            // I chose a pluralized naming scheme for Blockfrost.Api.Services
            services.AddScoped<Services.IWalletService, Services.WalletService>();          
            services.AddScoped<IKeyService, KeyService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IWalletStore, WalletStore>();

            // add the ViewModel, but as a Transient, which means it will create a new one each time.
            services.AddTransient<WalletViewModel>();
        }
    }
}
