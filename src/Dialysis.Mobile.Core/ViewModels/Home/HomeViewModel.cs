using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dialysis.Mobile.Core.ViewModels.Home
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IBluetoothLE ble;
        private readonly IAdapter adapter;

        public HomeViewModel(IBluetoothLE ble, IAdapter adapter)
        {
            this.ble = ble;
            this.adapter = adapter;
        }
    }
}
