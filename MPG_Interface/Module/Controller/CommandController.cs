using MPG_Interface.Module.Data.Output;
using MPG_Interface.Module.Data.Input;
using MPG_Interface.Module.Visual;
using MPG_Interface.Module.Logic;
using MPG_Interface.Module.Data;
using MPG_Interface.Xaml;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Linq;
using log4net;

namespace MPG_Interface.Module.Interfaces {

    /// <summary>
    /// Controller for manipulating data for the main window view
    /// </summary>
    public class CommandController {

        /// <summary>
        /// Reference to the data grid for showing the commands
        /// </summary>
        private readonly DataGrid dataGrid;

        /// <summary>
        /// Reference to the start date picker
        /// </summary>
        private readonly DatePicker start;

        /// <summary>
        /// Reference to the end date picker
        /// </summary>
        private readonly DatePicker end;

        /// <summary>
        /// Reference to the combo box with the statuses
        /// </summary>
        private readonly ComboBox status;

        /// <summary>
        /// Reference to the initial list of elements
        /// </summary>
        private List<ProductionOrder> oldList;

        /// <summary>
        /// Constructor that sets the data and the events
        /// </summary>
        /// <param name="local">List with the needed references</param>
        public CommandController(List<object> local) {
            dataGrid = local[0] as DataGrid;
            start = local[1] as DatePicker;
            end = local[2] as DatePicker;
            status = local[3] as ComboBox;

            ILog log = FactoryData.GetLogger();
            log.Debug("This is a Debug message");
            log.Info("This is a Info message");
            log.Warn("This is a Warning message");
            log.Error("This is an Error message");
            log.Fatal("This is a Fatal message");
            SetEvents();
        }

        /// <summary>
        /// Sets the needed events to the objects
        /// </summary>
        private void SetEvents() {
            dataGrid.CellEditEnding += async (sender, args) => {
                ProductionOrder order = args.Row.Item as ProductionOrder;
                DataGridCellInfo cellInfo = dataGrid.SelectedCells[9];
                string priority = (cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox).Text;

                // Check if the priority field has some data or it's only numbers
                if (string.IsNullOrEmpty(priority) || !priority.All(char.IsDigit)) {
                    Alerts.ShowMessage("Verificati valoarea introdusa");
                    ResetDataGrid(order);
                    return;
                }

                if (await RestClient.Client.CheckPriority(priority) && await CreateQC(order)) {
                    Alerts.ShowMessage("Prioritatea a fost setata");
                    return;
                }

                ResetDataGrid(order);
                Alerts.ShowMessage("Prioritatea nu a putut fi setata");
            };

            dataGrid.PreparingCellForEdit += (sender, args) => {
                DataGridCellInfo cellInfo = dataGrid.SelectedCells[9];
                ProductionOrder order = (ProductionOrder)cellInfo.Item;

                // Check is the command is done or blocked
                if (order.Status is not "ELB") {
                    Alerts.ShowMessage("Nu se mai poate seta prioritatea");
                    _ = dataGrid.CancelEdit();
                }
            };

            status.SelectionChanged += (sender, args) => {
                string orderStatus = (string)status.SelectedItem;
                dataGrid.ItemsSource = GetElementsByStatus(orderStatus);
            };

            dataGrid.LoadingRow += (sender, args) => {
                args.Row.Header = args.Row.GetIndex() + 1;
            };

            RestClient.Client.StartCall += () => {
                Application.Current.MainWindow.IsEnabled = false;
            };

            RestClient.Client.EndCall += () => {
                Application.Current.MainWindow.IsEnabled = true;
            };

            dataGrid.FontSize = 14;
            dataGrid.FontWeight = FontWeights.DemiBold;
        }

        private void ResetDataGrid(ProductionOrder order) {
            dataGrid.ItemsSource = null;
            order.Priority = null;
            dataGrid.ItemsSource = oldList;
        }

        private async Task<bool> CreateQC(ProductionOrder order) {
            string qc = await RestClient.Client.GetQC(order.POID);
            _ = StartCommand.CreateCommand(order.POID, qc, (int)order.PlannedQtyBUC);
            return qc != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        private List<ProductionOrder> GetElementsByStatus(string Status) {
            return string.IsNullOrEmpty(Status) ? oldList : oldList.Where(p => p.Status == Status).ToList();
        }

        /// <summary>
        /// Closes the command
        /// </summary>
        public async Task CloseCommand() {
            ProductionOrder po;
            if ((po = IsItemSelected()) == null) {
                return;
            }

            if (po.Status is not "PRLS") {
                Alerts.ShowMessage("Comanda nu poate fi inchisa");
                return;
            }

            if (!Alerts.ConfirmMessage("Sigur vreti sa inchideti comanda?")) {
                return;
            }

            string message = await RestClient.Client.CloseCommand(po.POID);
            if (string.IsNullOrEmpty(message)) {
                return;
            }

            UpdateStatus(po, "PRLT", "-1");
            Alerts.ShowMessage(message);
        }

        /// <summary>
        /// Used to create a partial report for the selected command
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task PartialReport() {
            ProductionOrder po;
            if ((po = IsItemSelected()) == null) {
                return;
            }

            if (po.Status is not "PRLS") {
                Alerts.ShowMessage("Nu se poate face predare partiala");
                return;
            }

            string message = await RestClient.Client.PartialProduction(po.POID);
            if (string.IsNullOrEmpty(message)) {
                return;
            }

            Alerts.ShowMessage(message);
        }

        /// <summary>
        /// Checks if an item is selected
        /// </summary>
        /// <returns>True if a command is selected <br/> False otherwise</returns>
        private ProductionOrder IsItemSelected() {
            if (dataGrid.SelectedItem == null) {
                Alerts.ShowMessage("Selectati o comanda!");
                return null;
            }

            return dataGrid.SelectedItem as ProductionOrder;
        }

        /// <summary>
        /// Used to block the selected command
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task BlockCommand() {
            ProductionOrder po;
            if ((po = IsItemSelected()) == null) {
                return;
            }

            if (!Alerts.ConfirmMessage("Sigur doriti sa blocati comanda?")) {
                return;
            }

            string message = await RestClient.Client.BlockCommand(po.POID);
            if (string.IsNullOrEmpty(message)) {
                return;
            }

            UpdateStatus(po, "BLOC", "-1");
            Alerts.ShowMessage(message);
        }

        /// <summary>
        /// Show the window used to set the quality control for a command
        /// </summary>
        public async Task ShowQualityWindow() {
            ProductionOrder po;
            if ((po = IsItemSelected()) == null) {
                return;
            }

            string poid = po.POID;
            int quantity = decimal.ToInt32((int)po.PlannedQtyBUC);
            string qc = await RestClient.Client.GetQC(poid);

            _ = new DetailsWindow(qc, poid, quantity) { Owner = Application.Current.MainWindow }.ShowDialog();
        }

        /// <summary>
        /// Sends the command to production
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task SendCommandToProduction() {
            ProductionOrder po;
            if ((po = IsItemSelected()) == null) {
                return;
            }

            if (po.Status != "ELB") {
                Alerts.ShowMessage("Comanda a fost deja transmisa");
                return;
            }

            StartCommand command = StartCommand.GetCommand(po.POID);
            command.Priority = int.Parse(po.Priority, CultureInfo.InvariantCulture);

            string message = await RestClient.Client.StartCommand(command);
            if (string.IsNullOrEmpty(message)) {
                return;
            }

            UpdateStatus(po, "PRLS", po.Priority);
            Alerts.ShowMessage(message);
        }

        /// <summary>
        /// Sets the active commands in the data grid
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task GetCommands() {
            var period = FactoryData.CreatePeriod(start.SelectedDate.Value, end.SelectedDate.Value);
            oldList = await RestClient.Client.GetCommands(period);
            dataGrid.ItemsSource = oldList;

            if (oldList?.Count == 0) {
                Alerts.ShowMessage("Nu exista comenzi in perioada selectata");
            }
        }

        /// <summary>
        /// Used to update the materials
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task UpdateMaterials() {
            string message = await RestClient.Client.DownloadMaterials();
            if (!string.IsNullOrEmpty(message)) {
                Alerts.ShowMessage(message);
            }
        }

        private void UpdateStatus(ProductionOrder po, string status, string priority) {
            po.Status = status;
            po.Priority = priority;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = oldList;
        }
    }
}
