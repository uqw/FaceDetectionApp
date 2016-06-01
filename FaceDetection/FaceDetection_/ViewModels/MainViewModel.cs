using GalaSoft.MvvmLight;
using System.Drawing;

namespace FaceDetection.ViewModels
{
    public class MainViewModel: ViewModelBase
    {
        public Bitmap Image { get; private set; }

        public MainViewModel()
        {
            // var capture = new Capture();
            // var frame = capture.QueryFrame();
            // Image = frame.Bitmap;
        }
    }
}
