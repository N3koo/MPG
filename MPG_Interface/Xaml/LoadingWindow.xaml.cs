using System.ComponentModel;
using System.Windows;

namespace MPG_Interface.Xaml {
    /// <summary>
    /// TODO: May be deleted
    /// </summary>
    public partial class LoadingWindow : Window {

        private static LoadingWindow _localLoading;

        private static bool _close;


        private LoadingWindow() {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = !_close;

            base.OnClosing(e);
        }

        public static void Show(string title) {
            if (_localLoading == null) {
                _localLoading = new LoadingWindow();
            }

            _close = false;
            _localLoading.Owner = Application.Current.MainWindow;
            _localLoading.Title = title;
            _localLoading.Show();
        }

        public static void CloseWindow() {
            _close = true;
            _localLoading.Close();
        }

    }
}
