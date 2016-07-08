using System;
using System.Configuration;
using System.Xml.Serialization;

namespace FaceDetection.Model
{
    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    [XmlRoot (ElementName = "CaptureResolution", IsNullable = false)]
    public class CaptureResolution
    {
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [XmlAttribute (AttributeName = "Width")]
        public int Width { get; set; }

        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [XmlAttribute(AttributeName = "Height")]
        public int Height { get; set; }

        public CaptureResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public CaptureResolution(): this(1024, 720)
        {
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
