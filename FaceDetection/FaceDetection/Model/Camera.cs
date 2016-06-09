namespace FaceDetection.Model
{
    /// <summary>
    /// Represents a camera.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Gets the index of the camera.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; }
        /// <summary>
        /// Gets the name of the camera.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="name">The name.</param>
        public Camera(int index, string name)
        {
            Index = index;
            Name = name;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return Name;
        }
    }
}