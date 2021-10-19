using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Forms;

namespace XamarinForms.BLE
{
    public partial class MainPage : ContentPage
    {
        private IAdapter _adapter;
        private IBluetoothLE _bluetoothBLE;
        private ObservableCollection<IDevice> _listDevices;
        private IDevice _device;

        public MainPage()
        {
            InitializeComponent();

            _bluetoothBLE = CrossBluetoothLE.Current;
            _adapter = CrossBluetoothLE.Current.Adapter;

            _listDevices = new ObservableCollection<IDevice>();
            DevicesList.ItemsSource = _listDevices;
        }

        private async void SearchDevice(object sender, EventArgs e)
        {
            if (_bluetoothBLE.State == BluetoothState.Off)
            {
                await DisplayAlert("Warning", "Bluetooth is turn off.", "OK");
            }
            else
            {
                _listDevices.Clear();
                _adapter.ScanTimeout = 10000;
                _adapter.ScanMode = ScanMode.Balanced;

                _adapter.DeviceDiscovered += (obj, a) =>
                {
                    if (!_listDevices.Contains(a.Device))
                    {
                        _listDevices.Add(a.Device);
                    }
                };

                await _adapter.StartScanningForDevicesAsync();
            }
        }

        private async void DevicesList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _device = DevicesList.SelectedItem as IDevice;

            var result = await DisplayAlert("Warning", "Do you like to connect with this device?", "Connect", "Cancel");

            if (!result)
                return;

            await _adapter.StopScanningForDevicesAsync();

            try
            {
                await _adapter.ConnectToDeviceAsync(_device);

                await DisplayAlert("Connected", "Status:" + _device.State, "OK");
            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}