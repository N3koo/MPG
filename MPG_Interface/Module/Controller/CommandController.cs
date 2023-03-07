using MPG_Interface.Module.Visual;
using MPG_Interface.Module.Logic;
using MPG_Interface.Module.Data;
using MPG_Interface.Xaml;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System;

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

            SetEvents();
        }

        /// <summary>
        /// Sets the needed events to the objects
        /// </summary>
        private void SetEvents() {
            dataGrid.CellEditEnding += async (sender, args) => {
                DataGridCellInfo cellInfo = dataGrid.SelectedCells[9];
                string priority = (cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox).Text;

                // Check if the priority field has some data or it's only numbers
                if (string.IsNullOrEmpty(priority) || !priority.All(char.IsDigit)) {
                    Alerts.ShowMessage("Verificati valoarea introdusa");
                    dataGrid.ItemsSource = oldList;
                    return;
                }

                if (await RestClient.Client.CheckPriority(priority)) {
                    Alerts.ShowMessage("Prioritatea a fost setata");
                } else {
                    Alerts.ShowMessage("Prioritatea nu a putut fi setata");
                    dataGrid.ItemsSource = oldList;
                }
            };

            dataGrid.PreparingCellForEdit += (sender, args) => {
                DataGridCellInfo cellInfo = dataGrid.SelectedCells[9];
                ProductionOrder order = (ProductionOrder)cellInfo.Item;

                // Check if priority is already set
                if (!string.IsNullOrEmpty(order.Priority)) {
                    Alerts.ShowMessage("Nu se mai poate seta o noua prioritate");
                    return;
                }

                // Check is the command is done or blocked
                if (order.Status != "ELB") {
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
            if (!IsItemSelected()) {
                return;
            }

            DataGridCellInfo cell = dataGrid.SelectedCells[9];
            string POID = (cell.Item as ProductionOrder).POID;

            if (await RestClient.Client.CloseCommand(POID)) {
                Alerts.ShowMessage("Commanda inchisa");
            }
        }

        /// <summary>
        /// Used to create a partial report for the selected command
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task PartialReport() {
            if (!IsItemSelected()) {
                return;
            }

            DataGridCellInfo cell = dataGrid.SelectedCells[9];
            string POID = (cell.Item as ProductionOrder).POID;

            if (await RestClient.Client.PartialProduction(POID)) {
                Alerts.ShowMessage("");
            }
        }

        /// <summary>
        /// Checks if an item is selected
        /// </summary>
        /// <returns>True if a command is selected <br/> False otherwise</returns>
        private bool IsItemSelected() {
            if (dataGrid.SelectedCells.Count == 0) {
                Alerts.ShowMessage("Selectati o comanda!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Used to block the selected command
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task BlockCommand() {
            if (!IsItemSelected()) {
                return;
            }

            if (!Alerts.ConfirmMessage("Sigur doriti sa blocati comanda?")) {
                return;
            }

            DataGridCellInfo cellInfo = dataGrid.SelectedCells[9];
            string poid = ((ProductionOrder)cellInfo.Item).POID;

            bool response = await RestClient.Client.BlockCommand(poid);
            if (!response) {
                Alerts.ShowMessage("Comanda nu a fost blocata!");
            } else {
                Alerts.ShowMessage("Comanda a fost blocata");
            }
        }

        /// <summary>
        /// Show the window used to set the quality control for a command
        /// TODO: CHeck this function
        /// </summary>
        public async Task ShowQualityWindow() {
            if (!IsItemSelected()) {
                return;
            }

            DataGridCellInfo cellInfo = dataGrid.SelectedCells[9];
            string poid = ((ProductionOrder)cellInfo.Item).POID;
            string qc = await RestClient.Client.GetQC(poid);

            new DetailsWindow(qc, poid) { Owner = Application.Current.MainWindow }.ShowDialog();
        }

        /// <summary>
        /// Sends the selected command to execution
        /// TODO: Check This function
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task SendCommandToProduction() {
            if (!IsItemSelected()) {
                return;
            }

            DataGridCellInfo cell = dataGrid.SelectedCells[9];
            string poid = ((ProductionOrder)cell.Item).POID;

            if (await RestClient.Client.StartCommand(poid)) {
                Alerts.ShowMessage("Comanda a fost transmisa!");
            }
        }

        /// <summary>
        /// Sets the active commands in the data grid
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task GetCommands() {
            oldList = await RestClient.Client.GetCommands(start.SelectedDate.Value, end.SelectedDate.Value);

            dataGrid.ItemsSource = oldList;
            if (oldList.Count == 0) {
                Alerts.ShowMessage("Nu exista comenzi in perioada selectata");
            }
        }

        /// <summary>
        /// Used to update the materials
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task UpdateMaterials() {
            if (await RestClient.Client.DownloadMaterials()) {
                Alerts.ShowMessage("Materialele au fost descarcate");
            }
        }
    }
}
