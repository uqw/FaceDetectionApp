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
        /// Gets the first name
        /// </summary>
        /// <value>
        /// The first name of the user
        /// </value>
        public string FirstName { get; }

        /// <summary>
        /// Gets the last name of the user
        /// </summary>
        /// <value>
        /// The last name of the user
        /// </value>
        public string LastName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="username">The username.</param>
        public User(int id, string username, string firstname, string lastname)
        {
            Id = id;
            Username = username;
            FirstName = firstname;
            LastName = lastname;
        }

        public User(int id, string username): this(id, username, "Unset", "Unset")
        {

        }
    }
}
