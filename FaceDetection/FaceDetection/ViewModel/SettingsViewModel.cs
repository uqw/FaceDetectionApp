using FaceDetection.Model.Recognition;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FaceDetection.ViewModel
{
    /// <summary>
    /// The settings view model
    /// </summary>
    public class SettingsViewModel: ViewModelBase
    {
        #region Fields
        private bool _restartNeeded;
        private readonly int _defaultExecutionDelay;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the scale factor front.
        /// </summary>
        /// <value>
        /// The scale factor front.
        /// </value>
        public double ScaleFactorFront
        {
            get { return Properties.Settings.Default.ScaleFactorFront; }
            set
            {
                RaisePropertyChanged(nameof(ScaleFactorFront));
                Properties.Settings.Default.ScaleFactorFront = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale factor profile.
        /// </summary>
        /// <value>
        /// The scale factor profile.
        /// </value>
        public double ScaleFactorProfile
        {
            get { return Properties.Settings.Default.ScaleFactorProfile; }
            set
            {
                RaisePropertyChanged(nameof(ScaleFactorProfile));
                Properties.Settings.Default.ScaleFactorProfile = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum neighbours.
        /// </summary>
        /// <value>
        /// The minimum neighbours.
        /// </value>
        public int MinNeighbours
        {
            get { return Properties.Settings.Default.MinNeighbours; }
            set
            {
                RaisePropertyChanged(nameof(MinNeighbours));
                Properties.Settings.Default.MinNeighbours = value;
            }
        }

        /// <summary>
        /// Gets or sets the execution delay.
        /// </summary>
        /// <value>
        /// The execution delay.
        /// </value>
        public int ExecutionDelay
        {
            get { return Properties.Settings.Default.ExecutionDelay; }
            set
            {
                Properties.Settings.Default.ExecutionDelay = value;
                RaisePropertyChanged(nameof(ExecutionDelay));

                UpdateRestartRequirement();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the restart button is shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the restart button is shown otherwise, <c>false</c>.
        /// </value>
        public bool RestartNeeded
        {
            get { return _restartNeeded; }
            set
            {
                _restartNeeded = value;
                RaisePropertyChanged(nameof(RestartNeeded));
            }
        }

        /// <summary>
        /// Gets the restart command.
        /// </summary>
        /// <value>
        /// The restart command.
        /// </value>
        public RelayCommand RestartCommand => new RelayCommand(Restart);

        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public int Radius
        {
            get { return Properties.Settings.Default.RecognitionRadius; }
            set
            {
                Properties.Settings.Default.RecognitionRadius = value;
                RaisePropertyChanged(nameof(Radius));
                ReinitializeRecognition();
            }
        }

        /// <summary>
        /// Gets or sets the neighbours.
        /// </summary>
        /// <value>
        /// The neighbours.
        /// </value>
        public int Neighbours
        {
            get { return Properties.Settings.Default.RecognitionNeighbours; }
            set
            {
                Properties.Settings.Default.RecognitionNeighbours = value;
                RaisePropertyChanged(nameof(Neighbours));
                ReinitializeRecognition();
            }
        }

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        /// <value>
        /// The threshold.
        /// </value>
        public double Threshold
        {
            get { return Properties.Settings.Default.RecognitionThreshold; }
            set
            {
                Properties.Settings.Default.RecognitionThreshold = value;
                RaisePropertyChanged(nameof(Threshold));
                ReinitializeRecognition();
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            _defaultExecutionDelay = Properties.Settings.Default.ExecutionDelay;
        }

        private void Restart()
        {
            App.RestartApp();
        }

        private void UpdateRestartRequirement()
        {
            RestartNeeded = (ExecutionDelay != _defaultExecutionDelay);
        }

        private void ReinitializeRecognition()
        {
            RecognitionEngine.InitializeFaceRecognizer();
        }
    }
}