using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;

namespace MPG_Interface.Module.Visual {
    public class DetailElement : TableRow {

        private readonly CheckBox _qcStatus;

        public DetailElement(bool isCheck, int index, string poid) {

            _qcStatus = new() {
                IsChecked = isCheck,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Cells.Add(new TableCell(new Paragraph(new Run($"{poid}_{index}"))) {
                TextAlignment = TextAlignment.Center
            });

            Cells.Add(new TableCell(new Paragraph(new Run($"ELB"))) {
                TextAlignment = TextAlignment.Center
            });

            Cells.Add(new TableCell(new BlockUIContainer(_qcStatus)));
        }

        public bool GetStatus() {
            return (bool)_qcStatus.IsChecked;
        }

        public void SetStatus(bool status) {
            _qcStatus.IsChecked = status;
        }
    }
}
