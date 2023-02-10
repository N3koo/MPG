using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

using MPG_Interface.Module.Controller;

namespace MPG_Interface.Xaml {
    /// <summary>
    /// Interaction logic for ConsumptionWindow.xaml
    /// </summary>
    public partial class ConsumptionWindow : Window {

        private static ConsumptionController _controller;

        public ConsumptionWindow() {
            InitializeComponent();

            SetEvents();
            CreateController(dgReport);
        }

        private void SetEvents() {
            btnClose.Click += (sender, args) => {
                Close();
            };
        }

        private static void CreateController(DataGrid dataGrid) {
            List<object> elements = new();
            elements.Add(dataGrid);
            _controller = new(elements);
        }

        public static void CreateConsumption(string POID) {
            new ConsumptionWindow().Show();
            _controller.SetDataCommand(POID);
        }

        public static void CreateConsumptionForPail(string POID, int pail) {
            new ConsumptionWindow().Show();
            _controller.SetDataPail(POID, pail);
        }
    }
}
