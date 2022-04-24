using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.OS;
using MvvmCross.Forms.Platforms.Android.Views;

namespace Dialysis.Mobile.Droid.Views
{
    [Activity(
       NoHistory = true,
       MainLauncher = true,
       Label = "@string/app_name",
       Theme = "@style/AppTheme.Splash",
       Icon = "@mipmap/ic_launcher",
       RoundIcon = "@mipmap/ic_launcher_round")]
    public class SplashActivity : MvxFormsSplashScreenActivity<Setup, Core.App, UI.App>
    {
        protected override Task RunAppStartAsync(Bundle bundle)
        {
            StartActivity(typeof(MainActivity));
            return Task.CompletedTask;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            UserDialogs.Init(this);
        }

    }
}
