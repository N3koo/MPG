using MPG_Interface.Module.Controller;

using System.Collections.Generic;
using System.Windows.Controls;
using System;

namespace MPG_Interface.Xaml.LocalControl {

    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl {

        private ReportController controller;

        public Report() {
            InitializeComponent();

            CreateController();
            SetDefaultValues();
            SetEvents();
        }

        private void CreateController() {
            List<object> elements = new() { dpReportStart, dpReportEnd, tgReport };
            controller = new(elements);
        }

        private void SetDefaultValues() {
            DateTime now = DateTime.Now;

            dpReportStart.SelectedDate = now;
            dpReportEnd.SelectedDate = now;
        }

        private void SetEvents() {
            btnShowReport.Click += async (sender, args) => {
                await controller.SetData();
            };

            miDetailReport.Click += async (sender, args) => {
                await controller.ShowDetails();
            };
        }
    }
}
