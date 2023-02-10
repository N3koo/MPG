﻿using System.Globalization;
using System.Windows.Data;
using System;

using MPG_Interface.Module.Visual.ViewModel;

using Syncfusion.UI.Xaml.TreeGrid.Cells;

namespace MPG_Interface.Module.Visual.Style {
    public class ToolTipGrid : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is TreeGridDataContextHelper) {
                return (value as TreeGridDataContextHelper).Value switch {
                    "ELB" => "COMANDA ELIBERATA",
                    "PRLT" => "CCOMANDA TERMINATA",
                    "PRLI" => "EROARE COMANDA",
                    "PRLS" => "COMANDA IN PRELUCRARE",
                    "BLOC" => "COMANDA BLOCATA",
                    _ => ""
                };
            }

            if (value is Report) {
                return (value as Report).Status switch {
                    "ELB" => "COMANDA ELIBERATA",
                    "PRLT" => "COMANDA TERMINATA",
                    "PRLI" => "EROARE COMANDA",
                    "PRLS" => "COMANDA IN PRELUCRARE",
                    "BLOC" => "COMANDA BLOCATA",
                    _ => ""
                };
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
}
