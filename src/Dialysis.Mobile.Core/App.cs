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
using System;
using Dialysis.Mobile.Core.ApiClient;
using Refit;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Dialysis.Mobile.Core.Services;
using Xamarin.Essentials;
using Dialysis.Mobile.Core.Utils;

namespace Dialysis.Mobile.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            //CreatableTypes()
            //    .EndingWith("Service")
            //    .AsInterfaces()
            //    .RegisterAsLazySingleton();
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
            Mvx.IoCProvider.RegisterType<IAuthService, AuthService>();
            InitializeServiceCollection();
            //RegisterAppStart<HomeViewModel>();
            RegisterCustomAppStart<AppStart>();
        }

        private static void InitializeServiceCollection()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            MapServiceCollectionToMvx(serviceProvider, serviceCollection);
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var apiUrl = Mvx.IoCProvider.Resolve<IConfiguration>()["ApiUrl"];
            // authService = Mvx.IoCProvider.Resolve<IAuthService>();
            serviceCollection.AddRefitClient<IDialysisAPI>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl))
                .AddPolicyHandler((provider, request) => 
                {
                    return Policy.HandleResult<System.Net.Http.HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        .RetryAsync(1, async (response, retryCount, context) =>
                        {
                            //await authService.RefreshToken();
                        });
                });
        }

        private static void MapServiceCollectionToMvx(IServiceProvider serviceProvider,
            IServiceCollection serviceCollection)
        {
            foreach (var serviceDescriptor in serviceCollection)
            {
                if (serviceDescriptor.ImplementationType != null)
                {
                    Mvx.IoCProvider.RegisterType(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType);
                }
                else if (serviceDescriptor.ImplementationFactory != null)
                {
                    var instance = serviceDescriptor.ImplementationFactory(serviceProvider);
                    Mvx.IoCProvider.RegisterSingleton(serviceDescriptor.ServiceType, instance);
                }
                else if (serviceDescriptor.ImplementationInstance != null)
                {
                    Mvx.IoCProvider.RegisterSingleton(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported registration type");
                }
            }
        }
    }
}

