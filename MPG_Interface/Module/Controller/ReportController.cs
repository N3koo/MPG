using System.Collections.Generic;
using System.Windows.Controls;

using MPG_Interface.Module.Visual.ViewModel;
using MPG_Interface.Xaml;

using Syncfusion.UI.Xaml.TreeGrid;

namespace MPG_Interface.Module.Controller {

    /// <summary>
    /// 
    /// </summary>
    public class ReportController {

        /// <summary>
        /// 
        /// </summary>
        private readonly DatePicker _startDate;

        /// <summary>
        /// 
        /// </summary>
        private readonly DatePicker _endDate;

        /// <summary>
        /// 
        /// </summary>
        private readonly SfTreeGrid _dataGrid;

        /// <summary>
        /// 
        /// </summary>
        private readonly ReportView _reportView;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public ReportController(List<object> list) {
            _startDate = list[0] as DatePicker;
            _endDate = list[1] as DatePicker;
            _dataGrid = list[2] as SfTreeGrid;

            _reportView = new();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetData() {
            _dataGrid.ItemsSource = _reportView.GetData(_startDate.SelectedDate, _endDate.SelectedDate);
        }

        public void ShowDetails() {
            Report item = _dataGrid.SelectedItem as Report;

            if (item.POID_ID != "-1") {
                int pail = int.Parse(item.POID.Split("_")[1]);
                ConsumptionWindow.CreateConsumptionForPail(item.POID_ID, pail);
            } else {
                ConsumptionWindow.CreateConsumption(item.POID);
            }
        }
    }
}
