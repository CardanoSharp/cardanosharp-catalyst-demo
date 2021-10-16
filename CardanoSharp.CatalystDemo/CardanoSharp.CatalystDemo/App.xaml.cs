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
            Startup.Init();
            InitializeComponent();
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
