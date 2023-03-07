using Syncfusion.UI.Xaml.TreeGrid;

using MPG_Interface.Module.Logic;
using MPG_Interface.Module.Data;
using MPG_Interface.Xaml;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;

namespace MPG_Interface.Module.Controller {

    /// <summary>
    /// 
    /// </summary>
    public class ReportController {

        /// <summary>
        /// 
        /// </summary>
        private readonly DatePicker startDate;

        /// <summary>
        /// 
        /// </summary>
        private readonly DatePicker endDate;

        /// <summary>
        /// 
        /// </summary>
        private readonly SfTreeGrid dataGrid;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public ReportController(List<object> list) {
            startDate = list[0] as DatePicker;
            endDate = list[1] as DatePicker;
            dataGrid = list[2] as SfTreeGrid;

            SetEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetEvents() {
            RestClient.Client.StartCall += () => {
                Application.Current.MainWindow.IsEnabled = false;
            };

            RestClient.Client.EndCall += () => {
                Application.Current.MainWindow.IsEnabled = true;
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task SetData() {
            dataGrid.ItemsSource = await RestClient.Client.GetReport(startDate.SelectedDate.Value, endDate.SelectedDate.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ShowDetails() {
            ReportCommand item = dataGrid.SelectedItem as ReportCommand;
            List<ReportMaterial> materials;

            if (item.POID_ID != "-1") {
                int pail = int.Parse(item.POID.Split("_")[1], CultureInfo.InvariantCulture);
                materials = await RestClient.Client.GetMaterialsForPail(item.POID_ID, pail);
            } else {
                materials = await RestClient.Client.GetMaterialsForCommand(item.POID);
            }

            ConsumptionWindow.CreateConsumption(materials);
        }
    }
}
