namespace FrameDetection.Model
{
    public class Camera
    {
        public int Index { get; }
        public string Name { get; }

        public Camera(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}