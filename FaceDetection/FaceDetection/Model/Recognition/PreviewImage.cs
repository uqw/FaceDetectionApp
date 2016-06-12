using Emgu.CV;
using Emgu.CV.Structure;

namespace FaceDetection.Model.Recognition
{
    public class PreviewImage
    {
        public Image<Bgr, byte> Original { get; }
        public Image<Gray, byte> Grayframe { get; }

        public PreviewImage(Image<Bgr, byte> original, Image<Gray, byte> grayframe)
        {
            Original = original;
            Grayframe = grayframe;
        }
    }
}
