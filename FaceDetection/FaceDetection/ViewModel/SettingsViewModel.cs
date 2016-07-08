using System.Collections.Generic;
using System.Collections.ObjectModel;
using FaceDetection.Model;
using FaceDetection.Model.Recognition;
using FaceDetection.ViewModel.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace FaceDetection.ViewModel
{
    /// <summary>
    /// The settings view model
    /// </summary>
    public class SettingsViewModel: ViewModelBase
    {
        private ObservableCollection<CaptureResolution> _resolutionList;
        private int _resolutionSelectionIndex;

        #region Fields
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
            }
        }

        public ObservableCollection<CaptureResolution> ResolutionList
        {
            get { return _resolutionList; }
            set
            {
                _resolutionList = value;
                RaisePropertyChanged(nameof(ResolutionList));
            }
        }

        public CaptureResolution ResolutionSelection
        {
            get { return Properties.Settings.Default.ResolutionSelection; }
            set
            {
                if (Properties.Settings.Default.ResolutionSelection != value)
                {
                    Properties.Settings.Default.ResolutionSelection = value;
                    RaisePropertyChanged(nameof(ResolutionSelection));
                    Messenger.Default.Send(new CaptureResolutionChangedMessage());
                }
            }
        }

        public int ResolutionSelectionIndex
        {
            get { return _resolutionSelectionIndex; }
            set
            {
                _resolutionSelectionIndex = value;
                RaisePropertyChanged(nameof(ResolutionSelectionIndex));
            }
        }

        #endregion

        public SettingsViewModel()
        {
            ResolutionList = new ObservableCollection<CaptureResolution>
            {
                new CaptureResolution(1920, 1080),
                new CaptureResolution(1024, 720),
                new CaptureResolution(640, 480),
                new CaptureResolution(480, 360),
                new CaptureResolution(256, 144)
            };

            ResolutionSelectionIndex = 0;
            ResolutionSelection = Properties.Settings.Default.ResolutionSelection;
        }

        public void ReinitializeRecognition()
        {
            RecognitionEngine.InitializeFaceRecognizer();
        }
    }
}