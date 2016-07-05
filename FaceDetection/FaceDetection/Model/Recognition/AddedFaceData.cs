namespace FaceDetection.Model.Recognition
{
    internal class AddedFaceData
    {
        public long UserId { get; }
        public long FaceId { get; }

        public AddedFaceData(long userId, long faceId)
        {
            UserId = userId;
            FaceId = faceId;
        }
    }
}
