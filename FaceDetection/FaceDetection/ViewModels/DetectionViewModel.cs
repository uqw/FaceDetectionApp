using System;
using Emgu.CV;
using System.Drawing;

namespace FaceDetection.ViewModels
{
    class DetectionViewModel
    {
        private Capture _capture;
        public Bitmap OutputImage { get; private set; }

        public DetectionViewModel()
        {
            try
            {
                _capture = new Capture();
                OutputImage = _capture.QueryFrame().Bitmap;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error retrieving image: " + ex);
            }
        }
    }
}
