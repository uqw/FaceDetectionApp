using Emgu.CV;
using Emgu.CV.Structure;

namespace FaceDetection.Model.Recognition
{
    /// <summary>
    /// Stores data for the preview image for the face detection
    /// </summary>
    public class PreviewImage
    {
        /// <summary>
        /// Gets the original picture.
        /// </summary>
        /// <value>
        /// The original.
        /// </value>
        public Image<Bgr, byte> Original { get; }

        /// <summary>
        /// Gets the grayframe.
        /// </summary>
        /// <value>
        /// The grayframe.
        /// </value>
        public Image<Gray, byte> Grayframe { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewImage"/> class.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="grayframe">The grayframe.</param>
        public PreviewImage(Image<Bgr, byte> original, Image<Gray, byte> grayframe)
        {
            Original = original;
            Grayframe = grayframe;
        }
    }
}
