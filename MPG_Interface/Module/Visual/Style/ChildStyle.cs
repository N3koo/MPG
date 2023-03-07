using MPG_Interface.Module.Data;

using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using System;

namespace MPG_Interface.Module.Visual.Style {

    /// <summary>
    /// 
    /// </summary>
    public class ChildStyle : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            StatusCommand order = value as StatusCommand;

            if (string.IsNullOrEmpty(order?.Name)) {
                return DependencyProperty.UnsetValue;
            }

            return order.Status switch {
                "PRLS" => DependencyProperty.UnsetValue,
                "BLOC" => new SolidColorBrush(Colors.LightBlue),
                "PRLT" => new SolidColorBrush(Colors.LightGreen),
                "PRLI" => new SolidColorBrush(Colors.Red),
                _ => DependencyProperty.UnsetValue,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
}
