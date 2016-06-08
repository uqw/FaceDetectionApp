using Emgu.CV;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrameDetection.Model;
using PostSharp.Patterns.Threading;

namespace FrameDetection.ViewModel
{
    class MainViewModel: ViewModelBase
    {
        private Bitmap _image;
        private int _selectedCam;
        private List<Camera> _availableCameras;
        private readonly CameraHandler _handler;

        public Bitmap Image
        {
            get
            {
                return _image;
            }
            private set
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

            private set
            {
                _availableCameras = value;
                RaisePropertyChanged(nameof(AvailableCameras));
            }
        }

        public MainViewModel()
        {
            _handler = new CameraHandler();
            AvailableCameras = _handler.GetAllCameras();
            RunCamViewer();
        }

        [Background]
        private void RunCamViewer()
        {
            while (true)
            {
                var cam = SelectedCam;
                Capture capture = null;

                try
                {
                    capture = new Capture(SelectedCam);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Couldn't read from camera input: " + AvailableCameras[SelectedCam]);
                }
                
                while (cam == SelectedCam)
                {
                    try
                    {
                        var frame = capture.QueryFrame();
                        if (frame.Bitmap != null)
                        {
                            Image = frame.Bitmap;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error reading frame: " + ex);
                    }

                    Thread.Sleep(20);
                }

                Debug.WriteLine("Camera selection changed: " + SelectedCam);
            }
        }
    }
}
