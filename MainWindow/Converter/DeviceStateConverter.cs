using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Managed.Adb;
using Orator.AoL.MainWindow.Models;

namespace Orator.AoL.MainWindow.Converter
{
    public class ConncectDeviceStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            { return false; }
            if (!(value is DeviceConncectState))
            {
                return false;
            }
            DeviceConncectState ds = (DeviceConncectState)value;
            if (ds == DeviceConncectState.NoDevice || ds == DeviceConncectState.CanDisconnect)
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DisconncectDeviceStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            { return false; }
            if (!(value is DeviceConncectState))
            {
                return false;
            }
            DeviceConncectState ds = (DeviceConncectState)value;
            if (ds == DeviceConncectState.NoDevice || ds == DeviceConncectState.CanConncect)
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
