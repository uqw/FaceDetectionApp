using FaceDetection.Model.Recognition;

namespace FaceDetection.ViewModel.Messages
{
    class FaceAddedMessage
    {
        public Face Face { get; }

        public FaceAddedMessage(Face face)
        {
            Face = face;
        }
    }
}
