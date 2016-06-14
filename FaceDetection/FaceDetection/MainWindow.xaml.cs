using System;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Controls;
using FaceDetection.ViewModel;
using FaceDetection.Model;

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

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var cameraViewModel = (CameraViewModel)CameraTab.DataContext;

                var dispatchTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
                dispatchTimer.Tick += (s, e) => { cameraViewModel.ReadFrame(); };
                dispatchTimer.Interval = new TimeSpan(0,0,0,0,40);
                dispatchTimer.Start();
            }
        }

        private void MetroAnimatedTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ProfileScaleSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            Settings.ScaleFactorProfile = 1 + slider.Value / 100;
            
        }

        private void FrontScaleSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            Settings.ScaleFactorFront = 1 + slider.Value / 100;
        }
    }
}
