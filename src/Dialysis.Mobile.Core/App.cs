using MvvmCross.IoC;
using MvvmCross.ViewModels;
using Dialysis.Mobile.Core.ViewModels.Home;
using MvvmCross;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;

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

            RegisterAppStart<HomeViewModel>();
        }
    }
}
