using System;
using System.ComponentModel;
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

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
