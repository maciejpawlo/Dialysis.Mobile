using Acr.UserDialogs;
using Dialysis.Mobile.Core.ApiClient;
using Dialysis.Mobile.Core.ApiClient.Responses;
using Dialysis.Mobile.Core.Models;
using Dialysis.Mobile.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Dialysis.Mobile.Core.ViewModels.Home
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly ILogger<HomeViewModel> logger;
        private readonly IBluetoothLE ble;
        private readonly IAdapter adapter;
        private readonly IUserDialogs dialogsService;
        private readonly IMvxNavigationService navigationService;
        private readonly IConfiguration configuration;
        private readonly IAuthService authService;
        private readonly IDialysisAPI dialysisAPI;
        private readonly IExaminationService examinationService;
        #region Properties
        public ObservableCollection<IDevice> DeviceList { get; set; }

        private bool _isScanButtonEnabled;
        public bool IsScanButtonEnabled
        {
            get => _isScanButtonEnabled;
            set => SetProperty(ref _isScanButtonEnabled, value);
        }

        private bool _isDeviceListVisible;
        public bool IsDeviceListVisible 
        {
            get => _isDeviceListVisible;
            set => SetProperty(ref _isDeviceListVisible, value);
        }

        public bool IsDeviceConnected
        {
            get => ConnectedDevice != null;
        }

        private IDevice _connectedDevice;
        public IDevice ConnectedDevice
        {
            get => _connectedDevice;
            set => SetProperty(ref _connectedDevice, value, async () => await RaiseAllPropertiesChanged());
        }

        public string DeviceName
        {
            get => ConnectedDevice?.Name;
        }
        #endregion

        #region Commands
        public MvxAsyncCommand ScanDevicesCommand { get; set; }
        public MvxAsyncCommand<Guid> ConnectToDeviceCommand { get; set; }
        public MvxAsyncCommand StartExamiantionCommand { get; set; }
        public MvxAsyncCommand DisconnectDeviceCommand { get; set; }
        public MvxAsyncCommand ScanAndConnectToDeviceCommand { get; set; }
        public MvxAsyncCommand LogoutCommand { get; set; }
        #endregion

        public HomeViewModel(ILogger<HomeViewModel> logger,
            IBluetoothLE ble,
            IAdapter adapter,
            IUserDialogs dialogsService,
            IMvxNavigationService navigationService,
            IConfiguration configuration,
            IAuthService authService,
            IDialysisAPI dialysisAPI,
            IExaminationService examinationService)
        {
            this.logger = logger;
            this.ble = ble;
            this.adapter = adapter;
            this.dialogsService = dialogsService;
            this.navigationService = navigationService;
            this.configuration = configuration;
            this.authService = authService;
            this.dialysisAPI = dialysisAPI;
            this.examinationService = examinationService;
            SetupCommands();
            Setup();
        }

        private async Task ScanAndConnectToDeviceAsync()
        {
            var result = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (result != PermissionStatus.Granted)
            {
                logger.LogInformation("Permission for LocationWhenInUse was not granted, requesting for permission");
                var requestResult = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (requestResult != PermissionStatus.Granted)
                {
                    logger.LogWarning("Permission request was denied");
                    return;
                }
            }

            if (!ble.IsOn)
            {
                logger.LogError("Could not start scanning, Bluetooth is off");
                await dialogsService.AlertAsync("Bluetooth connection is required. Turn on Bluetooth in settings.", "No Bluetooth connection", "OK");
                return;
            }

            logger.LogInformation("Started scanning for BLE devices");
            IsScanButtonEnabled = false;
            if (DeviceList.Count > 0) DeviceList.Clear();
            dialogsService.ShowLoading("Scanning devices...");
            await adapter.StartScanningForDevicesAsync(null, x => x.Name == configuration["DeviceName"]);
            dialogsService.HideLoading();
            logger.LogInformation("Finished scanning for BLE devices, found {count} devices", DeviceList.Count);
            //IsScanButtonEnabled = true;

            var deviceToConnect = DeviceList.FirstOrDefault();
            if (deviceToConnect is null)
            {
                logger.LogError("Could not connect to device, because no device was found");
                await dialogsService.AlertAsync("Could not connect to device, because no device was found.", "No device found", "OK");
                return;
            }

            try
            {
                logger.LogInformation($"Trying to connect to device (ID: {deviceToConnect.Id})");
                var connectedDevice = await adapter.ConnectToKnownDeviceAsync(deviceToConnect.Id);
                logger.LogInformation($"Successfully connected to device (ID: {deviceToConnect.Id})");
                IsDeviceListVisible = false;
                ConnectedDevice = connectedDevice;
                dialogsService.Toast($"Connected to {connectedDevice.Name}");
            }
            catch (DeviceConnectionException e)
            {
                logger.LogError($"Could not connect to device (ID: {deviceToConnect.Id})");
            }
            catch (Exception e)
            {
                logger.LogError($"Unknown error occurred while connecting to device (ID: {deviceToConnect.Id}), error: {e.Message}");
            }
        }

        private async Task ScanForDevicesAsync()
        {
            if (ble.IsOn)
            {
                logger.LogInformation("Started scanning for BLE devices");
                IsScanButtonEnabled = false;
                DeviceList.Clear();
                dialogsService.ShowLoading("Scanning devices...");
                await adapter.StartScanningForDevicesAsync(null, x => x.Name == configuration["DeviceName"]);
                dialogsService.HideLoading();
                logger.LogInformation("Finished scanning for BLE devices, found {count} devices", DeviceList.Count);
                IsScanButtonEnabled = true;

                //TODO: delete device list
                if (DeviceList.Count > 0)
                    IsDeviceListVisible = true;
            }
            else
            {
                await dialogsService.AlertAsync("Bluetooth connection is required. Turn on Bluetooth in settings.", "No Bluetooth connection", "OK");
            }
        }

        private async Task ConnectToDeviceAsync(Guid deviceGuid)
        {
            try
            {
                logger.LogInformation($"Trying to connect to device (ID: {deviceGuid})");
                var connectedDevice = await adapter.ConnectToKnownDeviceAsync(deviceGuid);
                logger.LogInformation($"Successfully connected to device (ID: {deviceGuid})");
                IsDeviceListVisible = false;
                ConnectedDevice = connectedDevice;
            }
            catch (DeviceConnectionException e)
            {
                logger.LogError($"Could not connect to device (ID: {deviceGuid})");
            }
            catch (Exception e)
            {
                logger.LogError($"Unknown error occurred while connecting to device (ID: {deviceGuid}), error: {e.Message}");
            }
        }

        private async Task DisconnectDeviceAsync()
        {
            logger.LogInformation($"Trying to disconnect device (ID: {ConnectedDevice.Id})");
            try
            {
                await adapter.DisconnectDeviceAsync(ConnectedDevice);
                logger.LogInformation($"Successfully disconnected device (ID: {ConnectedDevice.Id})");
                ConnectedDevice = null;
                IsScanButtonEnabled = true;
            }
            catch (Exception e)
            {
                logger.LogError($"Could not disconnect device (ID: {ConnectedDevice.Id}) due to unknown error: {e.Message}");
            }
        }

        private async Task StartExaminationAsync()
        {
            //TODOs:
            //read data - DONE
            //open popup with form regarding examination - DONE
            //send data to API - IN PROGRESS

            var services = await ConnectedDevice.GetServicesAsync();
            var primarySerivce = services.FirstOrDefault(x => x.Name == configuration["ServiceName"]);
            var characteristic = (await primarySerivce.GetCharacteristicsAsync()).FirstOrDefault();
            var sensorData = new List<double>();
            try
            {
                dialogsService.ShowLoading("Reading data from sensor...");

                logger.LogInformation($"Starting reading data from device (ID: {ConnectedDevice.Id})");
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                await RepeatActionEvery(async () => {
                    var data = await characteristic.ReadAsync();
                    var parsedData = Encoding.UTF8.GetString(data);
                    sensorData.Add(double.Parse(parsedData));
                }, TimeSpan.FromSeconds(1), cts.Token);
                logger.LogInformation($"Finished reading data from device (ID: {ConnectedDevice.Id}), data: {string.Join(", ",sensorData)}");
                dialogsService.HideLoading();
            }
            catch (Exception e)
            {
                logger.LogError($"Unknown error occurred while reading data from characteristic (Uuid: {characteristic.Uuid}) service (Name: {primarySerivce.Name}), device (ID: {ConnectedDevice.Id}), error: {e.Message}");
            }

            var examinationToSend = new Examination
            {
                CreatedAt = DateTime.Now,
                PatientID = 1, //TODO: To change when auth will be added
                Turbidity = sensorData.Average(),
            };

            var examination = await navigationService.Navigate<ExaminationResultViewModel, Examination, Examination>(examinationToSend);

            var examinationDTO = new ExaminationDTO
            {
                PatientID = examination.PatientID,
                Turbidity = examination.Turbidity,
                Weight = examination.Weight,
            };

            var result = await examinationService.CreateExaminations(examinationDTO);
        }

        private async Task RepeatActionEvery(Action action, TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                action();
                Task task = Task.Delay(interval, cancellationToken);

                try
                {
                    await task;
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private async Task Logout()
        {
            await authService.Logout();
            await navigationService.Navigate<LoginViewModel>();
        }

        private void OnDeviceDiscovered(object s, DeviceEventArgs a)
        {
            logger.LogInformation($"Discovered device, name: {a.Device.Name}, id: {a.Device.Id}");
            DeviceList.Add(a.Device);
        }

        private void SetupCommands()
        {
            ScanDevicesCommand = new MvxAsyncCommand(ScanForDevicesAsync);
            ConnectToDeviceCommand = new MvxAsyncCommand<Guid>((id) => ConnectToDeviceAsync(id));
            StartExamiantionCommand = new MvxAsyncCommand(StartExaminationAsync);
            DisconnectDeviceCommand = new MvxAsyncCommand(DisconnectDeviceAsync);
            ScanAndConnectToDeviceCommand = new MvxAsyncCommand(ScanAndConnectToDeviceAsync);
            LogoutCommand = new MvxAsyncCommand(Logout);
        }

        private void Setup()
        {
            IsScanButtonEnabled = true;
            DeviceList = new ObservableCollection<IDevice>();
            this.adapter.DeviceDiscovered += OnDeviceDiscovered;
        }
    }
}
