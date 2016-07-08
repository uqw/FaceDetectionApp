namespace FaceDetection.Model
{
    public class CaptureResolution
    {
        public int Width { get; }
        public int Height { get; }

        public CaptureResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() + Height.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var captureResolution = obj as CaptureResolution;
            if (captureResolution == null)
                return false;

            return captureResolution.Width == Width && captureResolution.Height == Height;
        }

        public override string ToString()
        {
            return $"{Width} x {Height}";
        }
    }
}
