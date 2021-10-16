using Android.Widget;
using CardanoSharp.CatalystDemo.Droid;
using CardanoSharp.CatalystDemo.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidToastService))]
namespace CardanoSharp.CatalystDemo.Droid
{
    public class AndroidToastService : IToast
    {
        public void ShortAlert(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }

        public void LongAlert(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }
    }
}