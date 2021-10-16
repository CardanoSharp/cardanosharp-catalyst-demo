using System;
using System.ComponentModel;
using System.Windows.Input;
using CardanoSharp.CatalystDemo.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CardanoSharp.CatalystDemo.Views
{
    public partial class WalletPage : ContentPage
    {
        public WalletPage()
        {
            InitializeComponent();
            // use our own DI Container to resolve the ViewModel
            try
            {
                this.BindingContext = Startup.ServiceProvider.GetRequiredService<WalletViewModel>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}