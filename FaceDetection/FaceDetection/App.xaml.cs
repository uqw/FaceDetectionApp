using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

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

        /// <summary>
        /// Restarts the application.
        /// </summary>
        public static void RestartApp()
        {
            try
            {
                System.Diagnostics.Process.Start(ResourceAssembly.Location);
                Current.Shutdown(0);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Could not restart app");
            }
            
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            FaceDetection.Properties.Settings.Default.Save();
            Environment.Exit(0);
        }
    }
}
