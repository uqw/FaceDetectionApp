using System;
using FaceDetection.Model;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Emgu.CV;
using FaceDetection.Model.Recognition;
using FaceDetection.ViewModel.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FaceDetection.ViewModel
{
    class CameraViewModel: ViewModelBase, IDisposable
    {
        #region Locals
        private bool _detectionEnabled;
        private bool _isAddFacePanelOpen;
        private CameraHandler.ProcessType _processType;
        private ObservableCollection<Camera> _availableCameras;
        private Bitmap _image;
        private int _selectedCam;
        private readonly CameraHandler _cameraHandler;
        private int _fps;
        private readonly Stopwatch _fpsStopwatch;
        private readonly Stopwatch _delayStopwatch;
        private bool _tabActive = true;
        private long _delay;

        public static Capture Capture;
        #endregion

        #region Properties
        public Bitmap Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
                RaisePropertyChanged(nameof(System.Drawing.Image));
            }
        }

        public int SelectedCam
        {
            get
            {
                return _selectedCam;
            }

            set
            {
                if (_selectedCam != value)
                {
                    _selectedCam = value;
                    RaisePropertyChanged(nameof(SelectedCam));
                    Properties.Settings.Default.SelectedCam = value;
                    Properties.Settings.Default.Save();

                    Image = null;
                    Capture.Dispose();
                    if (value != -1)
                    {
                        Capture = _cameraHandler?.CreateCapture(value);
                    }
                }
            }
        }

        public ObservableCollection<Camera> AvailableCameras
        {
            get
            {
                return _availableCameras;
            }

            set
            {
                _availableCameras = value;
                RaisePropertyChanged(nameof(AvailableCameras));
            }
        }

        public int Fps
        {
            get { return _fps; }

            set
            {
                _fps = value;
                RaisePropertyChanged(nameof(Fps));
            }
        }

        public CameraHandler.ProcessType ProcessType
        {
            get { return _processType; }

            set
            {
                _processType = value;
                RaisePropertyChanged(nameof(CameraHandler.ProcessType));
            }
        }

        public bool DetectionEnabled
        {
            get { return _detectionEnabled; }

            set
            {
                _detectionEnabled = value;
                RaisePropertyChanged(nameof(DetectionEnabled));
                Properties.Settings.Default.DetectionEnabled = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool IsAddFacePanelOpen
        {
            get { return _isAddFacePanelOpen; }

            set
            {
                if (_isAddFacePanelOpen != value)
                {
                    _isAddFacePanelOpen = value;
                    RaisePropertyChanged(nameof(IsAddFacePanelOpen));
                }
            }
        }

        public long Delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                RaisePropertyChanged(nameof(Delay));
            }
        }

        public RelayCommand RefreshCamerasCommand => new RelayCommand(RefreshCameras);
        public RelayCommand AddFaceCommand => new RelayCommand(AddFace);
        #endregion

        public CameraViewModel()
        {
            if(IsInDesignMode)
                return;

            SelectedCam = Properties.Settings.Default.SelectedCam;
            DetectionEnabled = Properties.Settings.Default.DetectionEnabled;

            Fps = 0;
            ProcessType = CameraHandler.ProcessType.Front;

            _cameraHandler = new CameraHandler();
            Capture = _cameraHandler.CreateCapture(SelectedCam);
            _fpsStopwatch = Stopwatch.StartNew();
            _delayStopwatch = new Stopwatch();

            InitializeMessageHandlers();
            RefreshCameras();
        }

        #region Methods

        private void InitializeMessageHandlers()
        {
            Messenger.Default.Register<TabSelectionChangedMessage>(this,
            (message) =>
            {
                _tabActive = message.Index == 0;
            });
        }

        /// <summary>
        /// Reads a frame of the camera.
        /// </summary>
        public void ReadFrame()
        {
            if (SelectedCam != -1 && _tabActive)
            {
                _delayStopwatch.Restart();
                Image = DetectionEnabled ? _cameraHandler.ProcessImage(Capture, ProcessType) : Capture?.QueryFrame().Bitmap;
                _fps++;

                if (_fpsStopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond > 250)
                {
                    Delay = _delayStopwatch.ElapsedMilliseconds;
                    _fpsStopwatch.Restart();
                    Fps = _fps * 4;
                    _fps = 0;
                }
            }
        }

        private void RefreshCameras()
        {
            AvailableCameras = new ObservableCollection<Camera>(_cameraHandler.GetAllCameras());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            _cameraHandler?.Dispose();
            Capture?.Dispose();
        }

        private void AddFace()
        {
            IsAddFacePanelOpen = !IsAddFacePanelOpen;
        }

        #endregion Methods
    }
}
