using MPG_Interface.Module.Controller;
using MPG_Interface.Module.Interfaces;

using System.Collections.Generic;
using System.Windows;
using System;

namespace MPG_Interface.Xaml {

    /// <summary>
    /// Class that implements the events for the main window
    /// </summary>
    public partial class MainWindow : Window {

        /// <summary>
        /// Used to implement the controls for the main window
        /// </summary>
        private readonly MainWindowController _mainController;

        /// <summary>
        /// Used to implement the manufacturing control
        /// </summary>
        private readonly ManufacturingStatusController _manufacturingController;

        /// <summary>
        /// Used to implement the report controlls
        /// </summary>
        private readonly ReportController _reportController;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow() {
            InitializeComponent();

            List<object> listMain = new() { dgShowData, dpStart, dpEnd, cbStatus };
            List<object> listStatus = new() { dpStartFollow, dpEndFollow, tgStatus };
            List<object> listReport = new() { dpReportStart, dpReportEnd, tgReport };

            _mainController = new MainWindowController(listMain);
            _manufacturingController = new ManufacturingStatusController(listStatus);
            _reportController = new ReportController(listReport);

            SetDefaultValues();
            SetEvents();
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        private void SetDefaultValues() {
            DateTime now = DateTime.Now;

            dpStart.SelectedDate = now;
            dpEnd.SelectedDate = now;

            dpStartFollow.SelectedDate = now;
            dpEndFollow.SelectedDate = now;

            dpReportStart.SelectedDate = now;
            dpReportEnd.SelectedDate = now;

            cbStatus.ItemsSource = new string[] { "BLOC", "PRLS", "PRLI", "PRLT", "ELB", "" };
        }

        /// <summary>
        /// Setting the events for the buttons
        /// </summary>
        private void SetEvents() {

            // After the window is loaded it requests to get the materials 
            mainWindow.Loaded += (sender, args) => {
                _mainController.InitalDownload();
            };

            // Sends the command to the production
            btnStart.Click += async (sender, args) => {
                await _mainController.SendCommandToProduction();
            };

            // Update materials
            btnUpdate.Click += async (sender, args) => {
                await _mainController.UpdateMaterials();
            };

            // Getting the commands
            btnShow.Click += async (sender, args) => {
                await _mainController.GetCommands();
            };

            // Settings window
            btnSettings.Click += (sender, args) => {
                new SettingsWindow().Show();
            };

            // Setting the QC for the pails
            btnDetails.Click += (sender, args) => {
                _mainController.ShowQualityWindow();
            };

            // Blocking a command
            btnBlock.Click += async (sender, args) => {
                await _mainController.BlockCommand();
            };

            // Closing a command
            btnClose.Click += async (sender, args) => {
                await _mainController.CloseCommand();
            };

            // Making a partial consumption
            btnPartial.Click += async (sender, args) => {
                await _mainController.PartialReport();
            };

            // Button for showing data in the following window
            btnSelect.Click += (sender, args) => {
                _manufacturingController.SetData();
            };

            // Used to show the report
            btnShowReport.Click += (sender, args) => {
                _reportController.SetData();
            };

            // Used to block a command
            miBlock.Click += async (sender, args) => {
                await _mainController.BlockCommand();
            };

            miClose.Click += async (sender, args) => {
                await _mainController.CloseCommand();
            };

            miSend.Click += async (sender, args) => {
                await _mainController.SendCommandToProduction();
            };

            miPartial.Click += async (sender, args) => {
                await _mainController.PartialReport();
            };

            miDetailReport.Click += (sender, args) => {
                _reportController.ShowDetails();
            };


            // Show a window with the explication for the messages
            // TODO: Maybe will be removed
            /*btnInfo.Click += (sender, args) => {
                string message = string.Format("{0}\n{1}\n{2}\n{3}",
                    "ELB - COMANDA ELIBERATA",
                    "PRLS - COMANDA IN PRELUCRARE",
                    "PRLI - EROARE COMANDA",
                    "PRLT - COMANDA TERMINATA");
                _ = MessageBox.Show(message);
            };/**/
        }
    }
}
