using MPG_Interface.Module.Visual;

using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System;

namespace MPG_Interface.Xaml {

    public partial class DetailsWindow : Window {

        private readonly List<DetailElement> listElements = new();

        private CheckBox qcAll;

        private bool[] local;

        private bool[] original;

        private string poid;

        public DetailsWindow(string qc, string poid) {
            InitializeComponent();

            SetCommandDetails(qc, poid);
        }

        private void SetCommandDetails(string qc, string poid) {
            this.poid = poid;

            CreateRows();
            SetDetails(qc);
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

            qcAll = new() {
                VerticalAlignment = VerticalAlignment.Center
            };

            _ = panel.Children.Add(label);
            _ = panel.Children.Add(qcAll);

            CurrentRow.Cells.Add(new TableCell(new BlockUIContainer(panel)));
        }

        private void SetDetails(string qc) {
            for (int i = 0; i < 18; i++) {
                DetailElement element = new(true, i + 1, poid);
                listElements.Add(element);
                tbDetails.RowGroups[0].Rows.Add(element);
            }
        }

        private void SetEvents() {
            Closed += (sender, args) => {
                int size = listElements.Count;
                /*for (int i = 0; i < size; i++) {
                    local[i] = listElements[i].GetStatus();
                }/**/
                //InputDataCollection.SetQC(_local, _poid);

                Close();
            };

            qcAll.Click += (sender, args) => {
                bool check = (bool)qcAll.IsChecked;
                listElements.ForEach(item => item.SetStatus(check));
            };

        }
    }
}
