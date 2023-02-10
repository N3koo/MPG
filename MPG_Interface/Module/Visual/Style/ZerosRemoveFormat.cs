using System.Globalization;
using System.Windows.Data;
using System;

namespace MPG_Interface.Module.Visual.Style {

    public class ZerosRemoveFormat : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value as string).TrimStart('0');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
