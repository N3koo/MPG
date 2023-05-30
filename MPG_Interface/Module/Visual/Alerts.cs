using System.Windows;
using System;

namespace MPG_Interface.Module.Visual {

    public class Alerts {

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

        public static void ShowError(string message) {
            _ = MessageBox.Show(Application.Current.MainWindow, message, "",
                MessageBoxButton.OK, MessageBoxImage.Error);
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

        public static bool ConfirmMessageThread(string message) {
            return (bool)Application.Current.Dispatcher.Invoke(_showMessageBox, message);
        }
    }
}
