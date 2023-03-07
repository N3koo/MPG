using MPG_Interface.Module.Data;

using System.Globalization;
using System.Windows.Data;
using System;

namespace MPG_Interface.Module.Visual.Style {
    public class ToolTipCustom : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            ProductionOrder order = value as ProductionOrder;
            return order.Status switch {
                "ELB" => "COMANDA ELIBERATA",
                "PRLT" => "COMANDA TERMINATA",
                "PRLI" => "EROARE COMANDA",
                "PRLS" => "COMANDA IN PRELUCRARE",
                "BLOC" => "COMANDA BLOCATA",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
