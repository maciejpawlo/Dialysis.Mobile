using Acr.UserDialogs;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
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
        public MvxAsyncCommand ScanForDevicesCommand { get; set; }
        public MvxAsyncCommand<Guid> ConnectToDeviceCommand { get; set; }
        public MvxAsyncCommand StartExamiantionCommand { get; set; }
        public MvxAsyncCommand DisconnectDeviceCommand { get; set; }
        #endregion

        public HomeViewModel(ILogger<HomeViewModel> logger,
            IBluetoothLE ble, 
            IAdapter adapter,
            IUserDialogs dialogsService)
        {
            this.logger = logger;
            this.ble = ble;
            this.adapter = adapter;
            this.dialogsService = dialogsService;
            SetupCommands();
            Setup();
        }

        private async Task ScanForDevicesAsync()
        {
            if (ble.IsOn)
            {
                logger.LogInformation("Started scanning for BLE devices");
                IsScanButtonEnabled = false;
                DeviceList.Clear();
                dialogsService.ShowLoading("Scanning devices...");
                await adapter.StartScanningForDevicesAsync();
                dialogsService.HideLoading();
                logger.LogInformation("Finished scanning for BLE devices, found {count} devices", DeviceList.Count);
                IsScanButtonEnabled = true;

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
            //open popup with form regarding examination
            //send data to API
            //do something with data 

            var services = await ConnectedDevice.GetServicesAsync();
            var primarySerivce = services.FirstOrDefault(x => x.Name == "Unknown Service");
            var characteristic = (await primarySerivce.GetCharacteristicsAsync()).FirstOrDefault();
            var textData = new List<string>();
            try
            {
                dialogsService.ShowLoading("Reading data from sensor...");

                logger.LogInformation($"Starting reading data from device (ID: {ConnectedDevice.Id})");
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                await RepeatActionEvery(async () => {
                    var data = await characteristic.ReadAsync();
                    var parsedData = Encoding.UTF8.GetString(data);
                    textData.Add(parsedData);
                }, TimeSpan.FromSeconds(1), cts.Token);
                logger.LogInformation($"Finished reading data from device (ID: {ConnectedDevice.Id}), data: {string.Join(", ",textData)}");
                dialogsService.HideLoading();
            }
            catch (Exception e)
            {
                logger.LogError($"Unknown error occurred while reading data from characteristic (Uuid: {characteristic.Uuid}) service (Name: {primarySerivce.Name}), device (ID: {ConnectedDevice.Id}), error: {e.Message}");
            }
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

        private void OnDeviceDiscovered(object s, DeviceEventArgs a)
        {
            logger.LogInformation($"Discovered device, name: {a.Device.Name}, id: {a.Device.Id}");
            DeviceList.Add(a.Device);
        }

        private void SetupCommands()
        {
            ScanForDevicesCommand = new MvxAsyncCommand(ScanForDevicesAsync);
            ConnectToDeviceCommand = new MvxAsyncCommand<Guid>((id) => ConnectToDeviceAsync(id));
            StartExamiantionCommand = new MvxAsyncCommand(StartExaminationAsync);
            DisconnectDeviceCommand = new MvxAsyncCommand(DisconnectDeviceAsync);
        }

        private void Setup()
        {
            IsScanButtonEnabled = true;
            DeviceList = new ObservableCollection<IDevice>();
            this.adapter.DeviceDiscovered += OnDeviceDiscovered;
        }
    }
}
