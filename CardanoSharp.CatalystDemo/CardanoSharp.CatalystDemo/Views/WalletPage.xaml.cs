using System;
using System.ComponentModel;
using System.Windows.Input;
using CardanoSharp.CatalystDemo.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CardanoSharp.CatalystDemo.Views
{
    public partial class WalletPage : ContentPage
    {
        public WalletPage()
        {
            InitializeComponent();
            this.BindingContext = new WalletViewModel();
        }    
    }
}