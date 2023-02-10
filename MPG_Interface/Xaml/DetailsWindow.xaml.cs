using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System;

using MPG_Interface.Module.Visual;

using DataEntity.Model.Types;

namespace MPG_Interface.Xaml {

    public partial class DetailsWindow : Window {

        private readonly List<DetailElement> _listElements = new();

        private CheckBox _qcAll;

        private bool[] _local;

        private bool[] _original;

        private string _poid;

        public DetailsWindow(string poid) {
            InitializeComponent();

            SetCommandDetails(poid);
        }

        private void SetCommandDetails(string poid) {
            _poid = poid;

            CreateRows();
            SetDetails();
            SetEvents();
        }

        private void CreateRows() {
            tbDetails.RowGroups.Add(new TableRowGroup());
            tbDetails.RowGroups[0].Rows.Add(new TableRow());
            TableRow CurrentRow = tbDetails.RowGroups[0].Rows[0];

            CurrentRow.Cells.Add(new TableCell(new Paragraph(new Run("POID_ID"))) {
                TextAlignment = TextAlignment.Center,

            });
            CurrentRow.Cells.Add(new TableCell(new Paragraph(new Run("Status"))) {
                TextAlignment = TextAlignment.Center
            });

            StackPanel panel = new() {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Label label = new() {
                Content = "QC",
            };

            _qcAll = new() {
                VerticalAlignment = VerticalAlignment.Center
            };

            _ = panel.Children.Add(label);
            _ = panel.Children.Add(_qcAll);

            CurrentRow.Cells.Add(new TableCell(new BlockUIContainer(panel)));
        }

        private void SetDetails() {
            _local = InputDataCollection.GetQC(_poid);
            int size = _local.Length;

            _original = new bool[size];
            Array.Copy(_local, _original, size);

            for (int i = 0; i < size; i++) {
                DetailElement local = new(_local[i], i + 1, _poid);
                _listElements.Add(local);
                tbDetails.RowGroups[0].Rows.Add(local);
            }
        }

        private void SetEvents() {
            Closed += (sender, args) => {
                int size = _listElements.Count;
                for (int i = 0; i < size; i++) {
                    _local[i] = _listElements[i].GetStatus();
                }
                InputDataCollection.SetQC(_local, _poid);

                Close();
            };

            _qcAll.Click += (sender, args) => {
                bool check = (bool)_qcAll.IsChecked;
                _listElements.ForEach(item => item.SetStatus(check));
            };

        }
    }
}
