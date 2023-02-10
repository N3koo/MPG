using MPG_Interface.Xaml;

using System.Windows;
using System;

namespace MPG_Interface.Module.Visual {

    public class Alerts {

        /// <summary>
        /// Event for showing or hiding the loading window
        /// </summary>
        private static readonly Action<bool, string> _showLoading = (bool status, string title) => {
            Application.Current.MainWindow.IsEnabled = status;

            if (status) {
                LoadingWindow.CloseWindow();
            } else {
                LoadingWindow.Show(title);
            }
        };

        /// <summary>
        /// 
        /// </summary>
        private static readonly Func<string, bool> _showMessageBox = (string message) => {
            return MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        };

        /// <summary>
        /// 
        /// </summary>
        private Alerts() {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void ShowMessage(string message) {
            _ = MessageBox.Show(Application.Current.MainWindow, message);
        }

        public static bool ConfirmMessage(string message) {
            return MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        public static void ShowLoading() {
            _ = Application.Current.Dispatcher.Invoke(_showLoading, false, "Asteptati descarcarea materialelor");
        }

        public static void HideLoading() {
            _ = Application.Current.Dispatcher.Invoke(_showLoading, true, null);
        }

        public static bool ConfirmMessageThread(string message) {
            return (bool)Application.Current.Dispatcher.Invoke(_showMessageBox, message);
        }

    }
}
