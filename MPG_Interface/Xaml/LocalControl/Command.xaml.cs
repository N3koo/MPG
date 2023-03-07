using MPG_Interface.Module.Interfaces;

using System.Collections.Generic;
using System.Windows.Controls;
using System;

namespace MPG_Interface.Xaml.LocalControl {

    /// <summary>
    /// Interaction logic for Command.xaml
    /// </summary>
    public partial class Command : UserControl {

        /// <summary>
        /// 
        /// </summary>
        private CommandController controller;

        /// <summary>
        /// 
        /// </summary>
        public Command() {
            InitializeComponent();

            CreateController();
            SetDefaultValues();
            SetEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateController() {
            List<object> list = new() { dgShowData, dpStart, dpEnd, cbStatus };
            controller = new(list);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetDefaultValues() {
            DateTime now = DateTime.Now;

            dpStart.SelectedDate = now;
            dpEnd.SelectedDate = now;

            cbStatus.ItemsSource = new string[] { "BLOC", "PRLS", "PRLI", "PRLT", "ELB", "" };
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetEvents() {
            // Sends the command to the production
            btnStart.Click += async (sender, args) => {
                await controller.SendCommandToProduction();
            };

            // Update materials
            btnUpdate.Click += async (sender, args) => {
                await controller.UpdateMaterials();
            };

            // Getting the commands
            btnShow.Click += async (sender, args) => {
                await controller.GetCommands();
            };

            // Settings window
            btnSettings.Click += (sender, args) => {
                _ = new SettingsWindow().ShowDialog();
            };

            // Setting the QC for the pails
            btnDetails.Click += async (sender, args) => {
                await controller.ShowQualityWindow();
            };

            // Blocking a command
            btnBlock.Click += async (sender, args) => {
                await controller.BlockCommand();
            };

            // Closing a command
            btnClose.Click += async (sender, args) => {
                await controller.CloseCommand();
            };

            // Making a partial consumption
            btnPartial.Click += async (sender, args) => {
                await controller.PartialReport();
            };

            // Used to block a command
            miBlock.Click += async (sender, args) => {
                await controller.BlockCommand();
            };

            // Close the command
            miClose.Click += async (sender, args) => {
                await controller.CloseCommand();
            };

            // Start command
            miSend.Click += async (sender, args) => {
                await controller.SendCommandToProduction();
            };

            // Send partial production
            miPartial.Click += async (sender, args) => {
                await controller.PartialReport();
            };
        }
    }
}
