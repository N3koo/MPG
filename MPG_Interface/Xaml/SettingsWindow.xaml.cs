using MPG_Interface.Module.Visual.ViewModel;

using System.Collections.Generic;
using System.Windows;

namespace MPG_Interface.Xaml {

    /// <summary>
    /// Used to show the settings window
    /// </summary>
    public partial class SettingsWindow : Window {

        /// <summary>
        /// List with the configuration elements
        /// </summary>
        private readonly List<SettingsElement> _list = new();

        /// <summary>
        /// Constructor that inserts the elements
        /// </summary>
        public SettingsWindow() {
            InitializeComponent();

            SetEvents();
            SetElements();
        }

        /// <summary>
        /// Sets the event for the visual elements
        /// </summary>
        private void SetEvents() {

            // Event for saving the data
            btnOK.Click += (sender, args) => {
                _list.ForEach(item => {
                    Properties.Settings.Default[item.Name] = item.DefaultValue;
                });

                Properties.Settings.Default.Save();
                Close();
            };

            // Event for closing the window
            btnCancel.Click += (sender, args) => {
                Close();
            };
        }

        /// <summary>
        /// Sets the elements in the window
        /// </summary>
        private void SetElements() {
            _list.Add(new SettingsElement { Name = "Plant", DefaultValue = Properties.Settings.Default.Plant });
            _list.Add(new SettingsElement { Name = "Code", DefaultValue = Properties.Settings.Default.Code });
            _list.Add(new SettingsElement { Name = "Input", DefaultValue = Properties.Settings.Default.Input });
            _list.Add(new SettingsElement { Name = "Connection", DefaultValue = Properties.Settings.Default.Connection });
            grid.ItemsSource = _list;
        }
    }
}
