using MPG_Interface.Module.Data.Output;
using MPG_Interface.Module.Visual;

using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;

namespace MPG_Interface.Xaml {

    public partial class DetailsWindow : Window {

        private readonly List<DetailElement> listElements = new();

        private CheckBox qcAll;

        StartCommand command;

        public DetailsWindow(string qc, string poid, int quantity) {
            InitializeComponent();

            SetCommandDetails(qc, poid, quantity);
        }

        private void SetCommandDetails(string qc, string poid, int quantity) {
            command = StartCommand.CreateCommand(poid, qc, quantity);

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

            qcAll = new() {
                VerticalAlignment = VerticalAlignment.Center
            };

            _ = panel.Children.Add(label);
            _ = panel.Children.Add(qcAll);

            CurrentRow.Cells.Add(new TableCell(new BlockUIContainer(panel)));
        }

        private void SetDetails() {
            /*local = new bool[quantity];
            original = new bool[quantity];

            for (int i = 0; i < original.Length; i++) {
                DetailElement element = new(local[i], i + 1, command.POID);
                listElements.Add(element);
                tbDetails.RowGroups[0].Rows.Add(element);
            }/**/
        }

        private void SetEvents() {
            Closed += (sender, args) => {
                int size = listElements.Count;
                /*for (int i = 0; i < size; i++) {
                    local[i] = listElements[i].GetStatus();
                }/**/

                Close();
            };

            qcAll.Click += (sender, args) => {
                bool check = (bool)qcAll.IsChecked;
                listElements.ForEach(item => item.SetStatus(check));
            };

        }
    }
}
