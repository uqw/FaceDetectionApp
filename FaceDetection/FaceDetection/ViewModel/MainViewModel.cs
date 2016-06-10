using Emgu.CV;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows;
using FaceDetection.Model;
using FaceDetection.Model.Recognition;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FaceDetection.ViewModel
{
    internal sealed class MainViewModel: ViewModelBase, IDisposable
    {
        #region Fields
        private Bitmap _image;
        private int _selectedCam;
        private ObservableCollection<Camera> _availableCameras;
        private readonly CameraHandler _cameraHandler;
        private int _fps;
        private readonly Stopwatch _fpsStopwatch;
        private CameraHandler.ProcessType _processType;
        private RecognitionData _recognitionData;
        private bool _detectionEnabled;
        private bool _isAddFaceFlyoutOpen;
        private Capture _capture;

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
                RaisePropertyChanged(nameof(Image));
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
                    _capture.Dispose();
                    if (value != -1)
                    {
                        _capture = _cameraHandler?.CreateCapture(value);
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
                RaisePropertyChanged(nameof(ProcessType));
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

        public bool IsAddFaceFlyoutOpen
        {
            get { return _isAddFaceFlyoutOpen; }

            set
            {
                if (_isAddFaceFlyoutOpen != value)
                {
                    _isAddFaceFlyoutOpen = value;
                    RaisePropertyChanged(nameof(IsAddFaceFlyoutOpen));
                }
            }
        }

        public RelayCommand RefreshCamerasCommand => new RelayCommand(RefreshCameras);
        public RelayCommand AddFaceCommand => new RelayCommand(AddFace);
        #endregion

        #region Construction
        public MainViewModel()
        {
            SelectedCam = Properties.Settings.Default.SelectedCam;
            DetectionEnabled = Properties.Settings.Default.DetectionEnabled;

            Fps = 0;
            ProcessType = CameraHandler.ProcessType.Front;

            _cameraHandler = new CameraHandler();
            _recognitionData = new RecognitionData();
            _capture = _cameraHandler.CreateCapture(SelectedCam);
            _fpsStopwatch = Stopwatch.StartNew();
            
            RefreshCameras();
        }
        #endregion

        #region Methods
        
        /// <summary>
        /// Reads a frame of the camera.
        /// </summary>
        public void ReadFrame()
        {
            if (SelectedCam != -1)
            {
                Image = DetectionEnabled ? _cameraHandler.ProcessImage(_capture, ProcessType) : _capture?.QueryFrame().Bitmap;
                _fps ++;

                if (_fpsStopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond > 250)
                {
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
            _cameraHandler.Dispose();
            _capture.Dispose();
        }

        private void AddFace()
        {
            IsAddFaceFlyoutOpen = !IsAddFaceFlyoutOpen;
        }
        #endregion
    }
}
