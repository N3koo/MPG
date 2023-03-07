using MPG_Interface.Module.Logic;

using Syncfusion.UI.Xaml.TreeGrid;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;

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
            tgStatus.ItemsSource = await RestClient.Client.GetStatusCommand(startDate.SelectedDate.Value,
                endDate.SelectedDate.Value);
        }
    }
}
