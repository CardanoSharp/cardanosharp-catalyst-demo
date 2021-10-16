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
            var bootstrap = new Bootstrapper(this);
            bootstrap.Start();
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
