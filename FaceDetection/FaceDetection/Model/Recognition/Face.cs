using Emgu.CV;
using Emgu.CV.Structure;

namespace FaceDetection.Model.Recognition
{
    public class Face
    {
        public Image<Bgr, byte> Original { get; }
        public Image<Gray, byte> Grayframe { get; }
        public int Id { get; }
        public string Username { get; }
        public int UserId { get; }

        public Face(byte[] original, byte[] grayframe, int id, string username, int userId, int width, int height)
        {
            Original = new Image<Bgr, byte>(width, height)
            {
                Bytes = original
            };

            Grayframe = new Image<Gray, byte>(width, height)
            {
                Bytes = grayframe
            };

            Id = id;
            Username = username;
            UserId = userId;
        }
    }
}
