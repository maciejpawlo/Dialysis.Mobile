using MvvmCross.IoC;
using MvvmCross.ViewModels;
using Dialysis.Mobile.Core.ViewModels.Home;
using MvvmCross;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using Acr.UserDialogs;

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

            Mvx.IoCProvider.RegisterSingleton<IBluetoothLE>(CrossBluetoothLE.Current);
            Mvx.IoCProvider.RegisterSingleton<IAdapter>(CrossBluetoothLE.Current.Adapter);
            Mvx.IoCProvider.RegisterSingleton<IUserDialogs>(UserDialogs.Instance);

            RegisterAppStart<HomeViewModel>();
        }
    }
}
