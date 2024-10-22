using MPG_Interface.Module.Data.Input;
using MPG_Interface.Module.Logic;
using MPG_Interface.Module.Visual;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MPG_Interface.Xaml
{

    /// <summary>
    /// Used to show the settings window
    /// </summary>
    public partial class SettingsWindow : Window {

        /// <summary>
        /// Constructor that inserts the elements
        /// </summary>
        public SettingsWindow() {
            InitializeComponent();

            SetEvents();
        }

        /// <summary>
        /// Sets the event for the visual elements
        /// </summary>
        private void SetEvents() {

            // Event for saving the data
            btnOK.Click += async (sender, args) => {
                List<SettingsElement> elements = (List<SettingsElement>)grid.ItemsSource;
                await RestClient.Client.SendSettings(elements);

                Close();
            };

            // Event for closing the window
            btnCancel.Click += (sender, args) => {
                Close();
            };

            Loaded += async (sender, args) => {
                await SetElements();
            };
        }

        /// <summary>
        /// Sets the elements in the window
        /// </summary>
        private async Task SetElements() {
            var settingsResponse = await RestClient.Client.GetSettings();

            if (settingsResponse?.Errors == null)
            {
                if (settingsResponse?.Data != null && settingsResponse.Data.Count > 0)
                    grid.ItemsSource = settingsResponse.Data;
                else
                    Alerts.ShowMessage("Nu s-a reusit incarcarea setarilor");
            }
            else
            {
                Alerts.ShowMessage(settingsResponse.Errors.Aggregate(new StringBuilder(), (current, next) => { return current.AppendLine($"Error [{next.Type}] - {next.Message}"); }).ToString());
            }
        }
    }
}
