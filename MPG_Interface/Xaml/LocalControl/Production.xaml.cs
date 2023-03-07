using MPG_Interface.Module.Controller;

using System.Windows.Controls;
using System;
using System.Collections.Generic;

namespace MPG_Interface.Xaml.LocalControl {
    /// <summary>
    /// Interaction logic for Production.xaml
    /// </summary>
    public partial class Production : UserControl {

        private ManufacturingStatusController controller;

        public Production() {
            InitializeComponent();

            CreateController();
            SetDefaultValues();
            SetEvents();
        }

        private void CreateController() {
            List<object> list = new() { dpStartFollow, dpEndFollow, tgStatus };
            controller = new(list);
        }

        private void SetDefaultValues() {
            DateTime now = DateTime.Now;

            dpStartFollow.SelectedDate = now;
            dpEndFollow.SelectedDate = now;
        }

        private void SetEvents() {
            btnSelect.Click += async (sender, args) => {
                await controller.SetData();
            };
        }
    }
}
