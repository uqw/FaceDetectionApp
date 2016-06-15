using Emgu.CV;
using Emgu.CV.Structure;

namespace FaceDetection.Model.Recognition
{
    /// <summary>
    /// Represents the information for a saved face for recognition
    /// </summary>
    public class Face
    {
        /// <summary>
        /// Gets the original picture.
        /// </summary>
        /// <value>
        /// The original picture.
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
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public User User { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// </summary>
        /// <param name="original">The original picture.</param>
        /// <param name="grayframe">The grayframe.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Face(byte[] original, byte[] grayframe, int id, string username, int userId, int width, int height): this
        (
            new Image<Bgr, byte>(width, height)
            {
                Bytes = original
            },

            new Image<Gray, byte>(width, height)
            {
                Bytes = grayframe
            },

            id,

            username,

            userId
        )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// </summary>
        /// <param name="original">The original picture.</param>
        /// <param name="grayframe">The grayframe.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="userId">The user identifier.</param>
        public Face(Image<Bgr, byte> original, Image<Gray, byte> grayframe, int id, string username, int userId)
        {
            Original = original;
            Grayframe = grayframe;
            Id = id;
            User = new User(userId, username);
            // TODO:  
        }
    }
}
