using CardanoSharp.CatalystDemo.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace CardanoSharp.CatalystDemo.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}