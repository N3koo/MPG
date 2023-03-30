using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

using MPG_Interface.Module.Data.Input;
using MPG_Interface.Module.Controller;

namespace MPG_Interface.Xaml {

    /// <summary>
    /// Interaction logic for ConsumptionWindow.xaml
    /// </summary>
    public partial class ConsumptionWindow : Window {

        /// <summary>
        /// Controller for the window
        /// </summary>
        private static ConsumptionController _controller;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConsumptionWindow() {
            InitializeComponent();

            SetEvents();
            CreateController(dgReport);
        }

        /// <summary>
        /// Setting the events
        /// </summary>
        private void SetEvents() {
            btnClose.Click += (sender, args) => {
                Close();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGrid"></param>
        private static void CreateController(DataGrid dataGrid) {
            List<object> elements = new() { dataGrid };
            _controller = new(elements);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="materials"></param>
        public static void CreateConsumption(List<ReportMaterial> materials) {
            new ConsumptionWindow().Show();
            _controller.SetData(materials);
        }
    }
}
