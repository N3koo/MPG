using MPG_Interface.Module.Logic;
using MPG_Interface.Module.Visual;
using MPG_Interface.Xaml;

using DataEntity.Model.Input;
using DataEntity.Model.Types;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Linq;

namespace MPG_Interface.Module.Interfaces {

    /// <summary>
    /// Controller for manipulating data for the main window view
    /// </summary>
    public class MainWindowController : IObserver {

        /// <summary>
        /// Reference to the data grid for showing the commands
        /// </summary>
        private readonly DataGrid _dataGrid;

        /// <summary>
        /// Reference to the start datepicker
        /// </summary>
        private readonly DatePicker _start;

        /// <summary>
        /// Reference to the end datepciker
        /// </summary>
        private readonly DatePicker _end;

        /// <summary>
        /// Reference to the combobox with the statuses
        /// </summary>
        private readonly ComboBox _status;

        /// <summary>
        /// Creates a new object that can be used to manipulate data
        /// </summary>
        private readonly IInput _input;

        /// <summary>
        /// Used to lock the update thread for the interface
        /// </summary>
        private bool _lock;

        /// <summary>
        /// Used to know if the edit is canceled
        /// </summary>
        private bool _cancelEdit;

        /// <summary>
        /// Constructor that sets the data and the events
        /// </summary>
        /// <param name="local">List with the needed references</param>
        public MainWindowController(List<object> local) {
            _dataGrid = (DataGrid)local[0];
            _start = (DatePicker)local[1];
            _end = (DatePicker)local[2];
            _status = (ComboBox)local[3];

            _cancelEdit = false;

            _input = Functions.CreateInput();
            _input.AddObserver(this);

            SetEvents();
            //CreateTimer();
        }

        /// <summary>
        /// Sets the needed events to the objects
        /// </summary>
        private void SetEvents() {
            _dataGrid.CellEditEnding += (sender, args) => {
                // Check if the edit event was canceled
                if (_cancelEdit) {
                    _lock = false;
                    _cancelEdit = false;
                    return;
                }

                DataGridCellInfo cellInfo = _dataGrid.SelectedCells[9];
                ProductionOrder order = (ProductionOrder)cellInfo.Item;
                string poid = order.POID;
                string priority = (cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox).Text;

                // Check if the priority field has some data or it's only numbers
                if (string.IsNullOrEmpty(priority) || !priority.All(char.IsDigit)) {
                    Alerts.ShowMessage("Verificati valoarea introdusa");
                    _dataGrid.ItemsSource = InputDataCollection.GetCommands();
                    _lock = false;
                    return;
                }

                if (InputDataCollection.CheckPriority(poid, priority)) {
                    Alerts.ShowMessage("Prioritatea a fost setata");
                } else {
                    Alerts.ShowMessage("Prioritatea nu a putut fi setata");
                    _dataGrid.ItemsSource = InputDataCollection.GetCommands();
                }

                _lock = false;
            };

            _dataGrid.PreparingCellForEdit += (sender, args) => {
                _lock = true;
                DataGridCellInfo cellInfo = _dataGrid.SelectedCells[9];
                ProductionOrder order = (ProductionOrder)cellInfo.Item;

                // Check if priority is already set
                if (!string.IsNullOrEmpty(order.Priority)) {
                    Alerts.ShowMessage("Nu se mai poate seta o noua prioritate");
                    _cancelEdit = true;
                    _ = _dataGrid.CancelEdit();
                    return;
                }

                // Check is the command is done or blocked
                if (order.Status is "PRLT" or "BLOC") {
                    Alerts.ShowMessage("Nu se mai poate seta statusul");
                    _cancelEdit = true;
                    _ = _dataGrid.CancelEdit();
                }
            };

            _status.SelectionChanged += (sender, args) => {
                _lock = true;
                string status = (string)_status.SelectedItem;
                _dataGrid.ItemsSource = InputDataCollection.GetCommandsByStatus(status);
                _lock = false;
            };

            _dataGrid.LoadingRow += (sender, args) => {
                args.Row.Header = args.Row.GetIndex() + 1;
            };

            _dataGrid.FontSize = 14;
            _dataGrid.FontWeight = FontWeights.DemiBold;
        }

        /// <summary>
        /// Creates and sets the timer event
        /// </summary>
        private void CreateTimer() {
            System.Timers.Timer _timer = new(500);
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, args) => {
                _input.VerifyProductionStatus();
            };

            _timer.Enabled = true;
        }

        /// <summary>
        /// Closes the command
        /// </summary>
        public async Task CloseCommand() {
            if (!IsItemSelected()) {
                return;
            }

            DataGridCellInfo cell = _dataGrid.SelectedCells[9];
            string POID = (cell.Item as ProductionOrder).POID;
            Application.Current.MainWindow.IsEnabled = false;
            await _input.CloseCommnadProduction(POID);
            Application.Current.MainWindow.IsEnabled = true;
            MessageBox.Show("Commanda inchisa");
        }

        /// <summary>
        /// Used to create a partiol report for the selected command
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task PartialReport() {
            if (!IsItemSelected()) {
                return;
            }

            DataGridCellInfo cell = _dataGrid.SelectedCells[9];
            string POID = (cell.Item as ProductionOrder).POID;

            Application.Current.MainWindow.IsEnabled = false;
            await _input.GeneratePartialProduction(POID);
            Application.Current.MainWindow.IsEnabled = true;
        }

        /// <summary>
        /// Checks if an item is selected
        /// </summary>
        /// <returns>True if a command is selected <br/> False otherwise</returns>
        private bool IsItemSelected() {
            if (_dataGrid.SelectedCells.Count == 0) {
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

            DataGridCellInfo cellInfo = _dataGrid.SelectedCells[9];
            string poid = ((ProductionOrder)cellInfo.Item).POID;
            if (_input.CommandExists(poid)) {
                Alerts.ShowMessage("Comanda se afla in productie si nu mai poate fi blocata");
                return;
            }

            bool status = await _input.SetCommandStatusAsync(poid, Properties.Resources.CMD_BLOCK);
            if (!status) {
                Alerts.ShowMessage("Command nu a fost blocata!");
            } else {
                Alerts.ShowMessage("Comanda a fost blocata");
            }
        }

        /// <summary>
        /// Show the window used to set the quality control for a command
        /// </summary>
        public void ShowQualityWindow() {
            if (!IsItemSelected()) {
                return;
            }

            DataGridCellInfo cellInfo = _dataGrid.SelectedCells[9];
            string poid = ((ProductionOrder)cellInfo.Item).POID;
            new DetailsWindow(poid).Show();
        }

        /// <summary>
        /// Sends the selected command to execution
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task SendCommandToProduction() {
            if (!IsItemSelected()) {
                return;
            }

            DataGridCellInfo cell = _dataGrid.SelectedCells[9];
            string poid = ((ProductionOrder)cell.Item).POID;
            switch (InputDataCollection.ExportCommand(poid)) {
                case -1:
                Alerts.ShowMessage("Comanda nu are prioritatea setata");
                return;
                case -2:
                Alerts.ShowMessage("Comanda selectata se afla deja in productie");
                return;
                default:
                break;
            }

            await _input.SetCommandStatusAsync(poid, Properties.Resources.CMD_PRLS);
            await GetCommands();
            Alerts.ShowMessage("Comanda a fost transmisa!");
        }

        /// <summary>
        /// Sets the active commands in the datagrid
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task GetCommands() {
            Application.Current.MainWindow.IsEnabled = false;
            await _input.GetCommandsAsync(_start.SelectedDate.Value, _end.SelectedDate.Value);
            _dataGrid.ItemsSource = InputDataCollection.GetCommands();
            if (_dataGrid.Items.Count == 0) {
                Alerts.ShowMessage("Nu exista comenzi in perioada selectata");
            }
            Application.Current.MainWindow.IsEnabled = true;
        }

        /// <summary>
        /// Download the initial materials for the first time or update them
        /// </summary>
        public void InitalDownload() {
            _input.GetMaterials();
        }

        /// <summary>
        /// Used to update the materials
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task UpdateMaterials() {
            await _input.UpdateMaterialsAsync();
        }

        /// <summary>
        /// Used to update the interface when a status is change
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task Update(string poid, string status) {
            while (_lock) {
                Thread.Sleep(100);
            }

            await _input.SetCommandStatusAsync(poid, status);
            InputDataCollection.SetStatus(poid, status);

            Application.Current.Dispatcher.Invoke(() => {
                _dataGrid.Items.Refresh();
            });
        }
    }
}
