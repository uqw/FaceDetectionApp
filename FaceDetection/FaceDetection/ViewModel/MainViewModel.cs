using Emgu.CV;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using Emgu.CV.Structure;
using FrameDetection.Model;
using GalaSoft.MvvmLight.Command;
using PostSharp.Patterns.Threading;
using Size = System.Drawing.Size;

namespace FrameDetection.ViewModel
{
    class MainViewModel: ViewModelBase
    {
        #region Fields
        private Bitmap _image;
        private int _selectedCam;
        private List<Camera> _availableCameras;
        private readonly CameraHandler _cameraHandler;
        private int _fps;
        private bool _progressBarShown;
        private CascadeClassifier _cascade;
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

        public bool ProgressBarShown
        {
            get { return _progressBarShown; }
            set
            {
                _progressBarShown = value;
                RaisePropertyChanged(nameof(ProgressBarShown));
            }
        }

        public RelayCommand RefreshCamerasCommand => new RelayCommand(RefreshCameras);
        #endregion

        #region Construction
        public MainViewModel()
        {
            SelectedCam = 0;
            Fps = 0;
            ProgressBarShown = true;

            _cameraHandler = new CameraHandler();

            InitializeFaceDetection();
            RefreshCameras();

            RunCamViewer();
        }
        #endregion

        #region Methods
        private void InitializeFaceDetection()
        {
            try
            {
                var cascadeFile = Path.GetTempFileName();
                File.WriteAllText(cascadeFile, Properties.Resources.haarcascade_alt_tree);

                _cascade = new CascadeClassifier(cascadeFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not write temp cascade file: " + ex);
            }
        }

        [Background]
        private void RunCamViewer()
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
                    ProgressBarShown = true;
                    try
                    {
                        capture = new Capture(SelectedCam);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Couldn't read from camera input: " + SelectedCam);
                    }
                }
                
                while (cam == SelectedCam)
                {
                    if(cam != -1)
                    {
                        try
                        {
                            ProgressBarShown = false;

                            /*
                            var frame = capture?.QueryFrame();
                            Image = frame?.Bitmap;
                            */

                            Image = ProcessImage(capture);

                            frames++;

                            if ((DateTime.Now.Subtract(timestamp).Ticks / TimeSpan.TicksPerMillisecond) > 1000)
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

                    Thread.Sleep(20);
                }

                capture?.Dispose();

                Debug.WriteLine("Camera selection changed: " + SelectedCam);
            }
        }

        private Bitmap ProcessImage(Capture capture)
        {
            if (capture == null || _cascade == null)
                return null;

            var imageFrame = capture.QueryFrame().ToImage<Bgr, Byte>();

            if (imageFrame == null)
                return null;

            var grayframe = imageFrame.Convert<Gray, byte>();

            // Detect the face
            var faces = _cascade.DetectMultiScale(grayframe, 1.25, 10, Size.Empty);

            foreach (var face in faces)
            {
                // Draw box around the face
                imageFrame.Draw(face, new Bgr(Color.BurlyWood));
            }

            return imageFrame.Bitmap;
        }

        private void RefreshCameras()
        {
            AvailableCameras = _cameraHandler.GetAllCameras();
        }

        #endregion
    }
}
