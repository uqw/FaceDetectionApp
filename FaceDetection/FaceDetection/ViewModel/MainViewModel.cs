using Emgu.CV;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using Emgu.CV.Structure;
using FaceDetection.Model;
using FaceDetection.Model.Recognition;
using GalaSoft.MvvmLight.Command;
using PostSharp.Patterns.Threading;
using Size = System.Drawing.Size;

namespace FaceDetection.ViewModel
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
        // private CascadeClassifier _cascadeFrontAltTree;
        // private CascadeClassifier _cascadeFrontAlt;
        private CascadeClassifier _cascadeFrontDefault;
        private readonly DataManager _dataManager;

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
            SelectedCam = Properties.Settings.Default.SelectedCam;
            Fps = 0;

            _cameraHandler = new CameraHandler();
            _dataManager = new DataManager();

            InitializeFaceDetection();
            RefreshCameras();

            new Thread(RunCamViewer).Start();
        }
        #endregion

        #region Methods
        private void InitializeFaceDetection()
        {
            try
            {
                var cascadeFile = Path.GetTempFileName();

                /*
                File.WriteAllText(cascadeFile, Properties.Resources.haarcascade_frontalface_alt_tree);
                _cascadeFrontAltTree = new CascadeClassifier(cascadeFile);
                
                File.WriteAllText(cascadeFile, Properties.Resources.haarcascade_frontalface_alt);
                _cascadeFrontAlt = new CascadeClassifier(cascadeFile);
                */

                File.WriteAllText(cascadeFile, Properties.Resources.haarcascade_frontalface_default);
                _cascadeFrontDefault = new CascadeClassifier(cascadeFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not write temp cascade file: " + ex);
            }
        }

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

                    Thread.Sleep(50);
                }

                capture?.Dispose();

                Debug.WriteLine("Camera selection changed: " + SelectedCam);
            }
        }

        private Bitmap ProcessImage(Capture capture)
        {
            if (capture == null || _cascadeFrontDefault == null)
                return null;

            var imageFrame = capture.QueryFrame().ToImage<Bgr, byte>();

            if (imageFrame == null)
                return null;

            var grayframe = imageFrame.Convert<Gray, byte>();

            // Detect the face
            // var facesAltTree = _cascadeFrontAltTree.DetectMultiScale(grayframe, 1.4, 10, Size.Empty);
            
            // var facesAlt = _cascadeFrontAlt.DetectMultiScale(grayframe, 1.4, 10, Size.Empty);

            var facesDefault = _cascadeFrontDefault.DetectMultiScale(grayframe, 1.25, 10, Size.Empty);

            /*
            foreach (var face in facesAltTree)
            {
                // Draw box around the face
                imageFrame.Draw(face, new Bgr(Color.BurlyWood));
            }

            foreach (var face in facesAlt)
            {
                imageFrame.Draw(face, new Bgr(Color.Aqua));
            }
            */

            foreach (var face in facesDefault)
            {
                imageFrame.Draw(face, new Bgr(Color.BlueViolet), 4);
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
