using MPG_Interface.Module.Data;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace MPG_Interface.Module.Controller {

    /// <summary>
    /// 
    /// </summary>
    public class ConsumptionController {

        /// <summary>
        /// 
        /// </summary>
        private readonly DataGrid grid;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public ConsumptionController(List<object> items) {
            grid = items[0] as DataGrid;

            SetEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetEvents() {
            grid.LoadingRow += (sender, args) => {
                args.Row.Header = args.Row.GetIndex() + 1;
            };

            grid.FontSize = 14;
            grid.FontWeight = FontWeights.DemiBold;
        }

        public void SetData(List<ReportMaterial> materials) {
            grid.ItemsSource = materials;
        }
    }
}
