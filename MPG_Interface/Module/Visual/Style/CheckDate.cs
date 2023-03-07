using MPG_Interface.Module.Data;

using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using System;

namespace MPG_Interface.Module.Visual.Style {

    /// <summary>
    /// Used to define the style when the date is not in the interval used in the report window
    /// </summary>
    public class CheckDate : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is not ReportCommand local || local.ExecuteDate == null) {
                return DependencyProperty.UnsetValue;
            }

            DateTime start = DateTime.ParseExact(local.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(local.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime now = DateTime.ParseExact(local.ExecuteDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            return start >= now && now <= end
                ? DependencyProperty.UnsetValue
                : new SolidColorBrush(Colors.LightPink);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
}
