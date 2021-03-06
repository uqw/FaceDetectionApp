﻿using System;
using FaceDetection.Model;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using FaceDetection.ViewModel.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FaceDetection.ViewModel
{
    internal class CameraViewModel: ViewModelBase, IDisposable
    {
        #region Locals
        private bool _detectionEnabled;
        private bool _isAddFacePanelOpen;
        private ObservableCollection<Camera> _availableCameras;
        private Bitmap _image;
        private int _selectedCam;
        private int _fps;
        private readonly Stopwatch _fpsStopwatch;
        private readonly Stopwatch _delayStopwatch;
        private bool _tabActive = true;
        private long _delay;

        public static Capture Capture;
        private bool _captureInProgress;
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
                if (_image != value)
                {
                    _image = value;
                    RaisePropertyChanged(nameof(Image));
                }
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
                    Capture?.Dispose();
                    if (value != -1)
                    {
                        Capture = CameraHandler?.CreateCapture(value);
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

        public static CameraHandler CameraHandler { get; private set; }

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

            CameraHandler = new CameraHandler();
            Capture = CameraHandler.CreateCapture(SelectedCam);
            Capture.SetCaptureProperty(CapProp.Fps, 30);
            Capture.ImageGrabbed += CaptureOnImageGrabbed;

            _fpsStopwatch = Stopwatch.StartNew();
            _delayStopwatch = new Stopwatch();

            InitializeMessageHandler();
            RefreshCameras();
        }

        private void CaptureOnImageGrabbed(object sender, EventArgs eventArgs)
        {
            ReadFrame();
        }

        #region Methods

        private void InitializeMessageHandler()
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
            if(_captureInProgress)
                return;

            if (SelectedCam != -1 && _tabActive && !MainViewModel.IsUpdating)
            {
                _captureInProgress = true;
                _delayStopwatch.Restart();
                Image = DetectionEnabled ? CameraHandler.ProcessImage(Capture) : Capture?.QueryFrame().Bitmap;
                _fps++;

                if (_fpsStopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond > 1000)
                {
                    Delay = _delayStopwatch.ElapsedMilliseconds;
                    _fpsStopwatch.Restart();
                    Fps = _fps;
                    _fps = 0;
                }

                _captureInProgress = false;
            }
            else
            {
                Fps = 0;
                Delay = 0;
                Image = null;
            }
        }

        private void RefreshCameras()
        {
            AvailableCameras = new ObservableCollection<Camera>(CameraHandler.GetAllCameras());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            CameraHandler?.Dispose();
            Capture?.Dispose();
        }

        private void AddFace()
        {
            IsAddFacePanelOpen = !IsAddFacePanelOpen;
        }

        #endregion Methods
    }
}
