using System;
using System.Collections.Generic;
using System.Windows.Controls;

using MPG_Interface.Module.Visual.ViewModel;

using Syncfusion.UI.Xaml.TreeGrid;

namespace MPG_Interface.Module.Controller {

    /// <summary>
    /// Controller for manipulating data for the status report
    /// </summary>
    public class ManufacturingStatusController {

        /// <summary>
        /// Reference to the start date
        /// </summary>
        private readonly DatePicker _startDate;

        /// <summary>
        /// Reference to the end date
        /// </summary>
        private readonly DatePicker _endDate;

        /// <summary>
        /// Reference to the tree grid
        /// </summary>
        private readonly SfTreeGrid _tgStatus;

        /// <summary>
        /// Object that implements to logic of the report
        /// </summary>
        private readonly OperationView _operation;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="list">List that contains the elements of the controller</param>
        public ManufacturingStatusController(List<object> list) {
            _startDate = (DatePicker)list[0];
            _endDate = (DatePicker)list[1];
            _tgStatus = (SfTreeGrid)list[2];

            _operation = new OperationView();
        }

        /// <summary>
        /// Sets the data in the table
        /// </summary>
        public void SetData() {
            _tgStatus.ItemsSource = _operation.GetData(_startDate.SelectedDate, _endDate.SelectedDate);
        }
    }
}
