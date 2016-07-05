using FaceDetection.Model.Recognition;

namespace FaceDetection.ViewModel.Messages
{
    internal class FaceAddedMessage
    {
        public Face Face { get; }

        public FaceAddedMessage(Face face)
        {
            Face = face;
        }
    }
}
