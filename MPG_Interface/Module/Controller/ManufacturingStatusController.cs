using MPG_Interface.Module.Logic;

using Syncfusion.UI.Xaml.TreeGrid;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;
using MPG_Interface.Module.Data;
using MPG_Interface.Module.Visual;
using System.Text;
using System.Linq;

namespace MPG_Interface.Module.Controller {

    /// <summary>
    /// Controller for manipulating data for the status report
    /// </summary>
    public class ManufacturingStatusController {

        /// <summary>
        /// Reference to the start date
        /// </summary>
        private readonly DatePicker startDate;

        /// <summary>
        /// Reference to the end date
        /// </summary>
        private readonly DatePicker endDate;

        /// <summary>
        /// Reference to the tree grid
        /// </summary>
        private readonly SfTreeGrid tgStatus;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="list">List that contains the elements of the controller</param>
        public ManufacturingStatusController(List<object> list) {
            startDate = (DatePicker)list[0];
            endDate = (DatePicker)list[1];
            tgStatus = (SfTreeGrid)list[2];

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
        /// Sets the data in the table
        /// </summary>
        public async Task SetData() {
            var period = FactoryData.CreatePeriod(startDate.SelectedDate.Value, endDate.SelectedDate.Value);
            var statusResponse = await RestClient.Client.GetStatusCommand(period);
            if (statusResponse?.Data != null)
                tgStatus.ItemsSource = statusResponse.Data;
            else if (statusResponse?.Errors != null)
                Alerts.ShowMessage(statusResponse?.Errors.Aggregate(new StringBuilder(), (current, next) => { return current.AppendLine($"Error [{next.Type}] - {next.Message}"); }).ToString());
            else
                Alerts.ShowMessage("Eroare necunoscuta");
        }
    }
}
