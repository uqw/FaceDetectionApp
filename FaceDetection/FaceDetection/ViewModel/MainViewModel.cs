using Emgu.CV;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using FaceDetection.Model;
using FaceDetection.Model.Recognition;
using GalaSoft.MvvmLight.Command;

namespace FaceDetection.ViewModel
{
    internal sealed class MainViewModel: ViewModelBase, IDisposable
    {
        #region Fields
        private Bitmap _image;
        private int _selectedCam;
        private List<Camera> _availableCameras;
        private readonly CameraHandler _cameraHandler;
        private int _fps;
        private readonly DataManager _dataManager;
        private CameraHandler.ProcessType _processType;
        private bool _detectionEnabled;
        private Thread _detectionThread;

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
                _selectedCam = value;
                RaisePropertyChanged(nameof(SelectedCam));
                Properties.Settings.Default.SelectedCam = value;
                Properties.Settings.Default.Save();
            }
        }

        public List<Camera> AvailableCameras
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

        public RelayCommand RefreshCamerasCommand => new RelayCommand(RefreshCameras);
        #endregion

        #region Construction
        public MainViewModel()
        {
            SelectedCam = Properties.Settings.Default.SelectedCam;
            DetectionEnabled = Properties.Settings.Default.DetectionEnabled;

            Fps = 0;
            ProcessType = CameraHandler.ProcessType.Front;

            _cameraHandler = new CameraHandler();
            _dataManager = new DataManager();
            
            RefreshCameras();
            StartDetection();
        }
        #endregion

        #region Methods

        private void StartDetection()
        {
            if (!IsInDesignMode)
            {
                _detectionThread = new Thread(RunCamViewer);
                _detectionThread.Start();
            }
        }

        private void RunCamViewer()
        {
            try
            {
                while (true)
                {
                    var cam = SelectedCam;
                    var frames = 0;
                    var timestamp = DateTime.Now;

                    Capture capture = null;
                    Image = null;
                    Fps = 0;

                    if (cam != -1)
                    {
                        capture = CreateCapture(cam);
                    }

                    while (cam == SelectedCam)
                    {
                        if (cam != -1)
                        {
                            try
                            {
                                Image = DetectionEnabled ? _cameraHandler.ProcessImage(capture, ProcessType) : capture?.QueryFrame().Bitmap;

                                frames++;

                                if (DateTime.Now.Subtract(timestamp).Ticks / TimeSpan.TicksPerMillisecond > 1000)
                                {
                                    Fps = frames;
                                    frames = 0;
                                    timestamp = DateTime.Now;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Error reading frame: " + ex);
                            }
                        }

                        Thread.Sleep(50);
                    }

                    capture?.Dispose();

                    Debug.WriteLine("Camera selection changed: " + SelectedCam);
                }
            }
            catch (ThreadInterruptedException)
            {
                // ignored
            }
        }

        private Capture CreateCapture(int selection)
        {
            if (selection > -1)
            {
                try
                {
                    return new Capture(SelectedCam);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Couldn't read from camera input: " + SelectedCam);
                }
            }

            return null;
        }

        private void RefreshCameras()
        {
            AvailableCameras = _cameraHandler.GetAllCameras();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            _cameraHandler.Dispose();
            _dataManager.Dispose();
        }
        #endregion
    }
}
