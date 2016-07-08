using System;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using FaceDetection.ViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            ((MainViewModel)DataContext).DialogCoordinator = DialogCoordinator.Instance;
            ((MainViewModel)DataContext).DialogCoordinatorRegistered = true;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var cameraViewModel = (CameraViewModel)CameraTab.DataContext;
                
                var dispatchTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
                dispatchTimer.Tick += (s, e) => { cameraViewModel.ReadFrame(); };
                dispatchTimer.Interval = new TimeSpan(0,0,0,0,Properties.Settings.Default.ExecutionDelay);
                dispatchTimer.Start();
            }
        }

        private void RecognitionSliderOnManipulationCompleted(object sender, DragCompletedEventArgs e)
        {
            ((SettingsViewModel)SettingsTab.DataContext).ReinitializeRecognition();
        }
    }
}
