using Syncfusion.UI.Xaml.TreeGrid;

using MPG_Interface.Module.Logic;
using MPG_Interface.Module.Data;
using MPG_Interface.Xaml;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using MPG_Interface.Module.Data.Input;
using MPG_Interface.Module.Data.Output;
using MPG_Interface.Module.Visual;
using System.Text;
using System.Linq;

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
            Period period = FactoryData.CreatePeriod(startDate.SelectedDate.Value, endDate.SelectedDate.Value);
            var reportResults = await RestClient.Client.GetReport(period);

            if (reportResults.Errors == null)
            {
                if (reportResults.Data != null && reportResults.Data.Count > 0)
                    dataGrid.ItemsSource = reportResults.Data;
                else
                    Alerts.ShowMessage("Nu exista rapoarte in perioada selectata");
            }
            else
            {
                Alerts.ShowMessage(reportResults.Errors.Aggregate(new StringBuilder(), (current, next) => { return current.AppendLine($"Error [{next.Type}] - {next.Message}"); }).ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ShowDetails() {
            ReportCommand item = dataGrid.SelectedItem as ReportCommand;
            ServiceResponse<List<ReportMaterial>> materialsResponse;

            if (item.POID_ID != "-1") {
                int pail = int.Parse(item.POID.Split("_")[1], CultureInfo.InvariantCulture);
                materialsResponse = await RestClient.Client.GetMaterialsForPail(item.POID_ID, pail);
            } else {
                materialsResponse = await RestClient.Client.GetMaterialsForCommand(item.POID);
            }

            if (materialsResponse?.Data != null)
                ConsumptionWindow.CreateConsumption(materialsResponse.Data);
            else if (materialsResponse?.Errors != null)
                Alerts.ShowMessage(materialsResponse?.Errors.Aggregate(new StringBuilder(), (current, next) => { return current.AppendLine($"Error [{next.Type}] - {next.Message}"); }).ToString());
            else
                Alerts.ShowMessage("Eroare necunoscuta");
        }
    }
}
