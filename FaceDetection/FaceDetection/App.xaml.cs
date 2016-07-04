using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using FaceDetection.Model;

namespace FaceDetection
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            try
            {
                new Mutex(false, "FaceDetectionApp");
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Could not register mutex");
            }

            try
            {
                FaceDetection.Properties.Settings.Default.Upgrade();
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Could not upgrade settings");
            }

            FaceDetection.Properties.Settings.Default.PropertyChanged += DefaultSettingsOnPropertyChanged;
        }

        private void DefaultSettingsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            FaceDetection.Properties.Settings.Default.Save();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            FaceDetection.Properties.Settings.Default.Save();
            Environment.Exit(0);
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MainWindow.Hide();
            Logger.CriticalError("App crashed: " + e.Exception);
            var result = MessageBox.Show("An error occured.\n\nDo you want to restart the app?", "Critical error", MessageBoxButton.YesNo,
                MessageBoxImage.Error);

            if(result == MessageBoxResult.Yes)
                RestartApp();

            Environment.Exit(0);
        }

        private void RestartApp()
        {
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
            Environment.Exit(0);
        }
    }
}
