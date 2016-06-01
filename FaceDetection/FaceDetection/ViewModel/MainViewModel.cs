using Emgu.CV;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameDetection.ViewModel
{
    class MainViewModel: ViewModelBase
    {
        private Bitmap _image;

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

        public MainViewModel()
        {
            Task.Run(
                () =>
                {
                    var capture = new Capture(1);
                    var frame = capture.QueryFrame();
                    Image = frame.Bitmap;
                });
            
        }
    }
}
