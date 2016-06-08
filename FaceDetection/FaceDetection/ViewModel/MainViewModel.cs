using Emgu.CV;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Patterns.Threading;

namespace FrameDetection.ViewModel
{
    class MainViewModel: ViewModelBase
    {
        private Bitmap _image;
        private int _selectedCam;

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

            private set
            {
                _selectedCam = value;
                RaisePropertyChanged(nameof(SelectedCam));
            }
        }

        public MainViewModel()
        {
            SelectedCam = 1;
            RunCamViewer();
        }

        [Background]
        private void RunCamViewer()
        {
            var cam = SelectedCam;
            while (true)
            {
                var capture = new Capture(SelectedCam);
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
                        System.Diagnostics.Debug.WriteLine("Error reading frame: " + ex);
                    }

                    Thread.Sleep(20);
                }
                cam = SelectedCam;
            }
        }
    }
}
