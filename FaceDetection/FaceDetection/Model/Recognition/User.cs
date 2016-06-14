namespace FaceDetection.Model.Recognition
{
    /// <summary>
    /// Represents a user saved in the database
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="username">The username.</param>
        public User(int id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}
