using MvvmCross.IoC;
using MvvmCross.ViewModels;
using Dialysis.Mobile.Core.ViewModels.Home;
using MvvmCross;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using Acr.UserDialogs;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Dialysis.Mobile.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
#if RELEASE
            var path = "Dialysis.Mobile.Core.Configuration.appsettings.release.json";
#else
            var path = "Dialysis.Mobile.Core.Configuration.appsettings.debug.json";
#endif
            var resourceStream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream(path);
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(resourceStream)
                .Build();

            Mvx.IoCProvider.RegisterSingleton<IConfiguration>(configuration);
            Mvx.IoCProvider.RegisterSingleton<IBluetoothLE>(CrossBluetoothLE.Current);
            Mvx.IoCProvider.RegisterSingleton<IAdapter>(CrossBluetoothLE.Current.Adapter);
            Mvx.IoCProvider.RegisterSingleton<IUserDialogs>(UserDialogs.Instance);

            RegisterAppStart<HomeViewModel>();
        }
    }
}
