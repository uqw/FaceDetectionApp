using System;
using System.Drawing;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using FaceDetection.Model;

namespace FaceDetection.Views.Converters
{
    internal class BitmapConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bitmap = value as Bitmap;
            if (bitmap == null)
            {
#if DEBUG
                Logger.Error("Converting error. Failed to cast input as Bitmap");
#endif
                return null;
            }

#if !DEBUG
            RuntimeHelpers.PrepareConstrainedRegions();
#endif
            try
            {
#if !DEBUG
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;
#endif
                var ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
#if DEBUG
                Logger.Error("Converting error. Failed to cast input as Bitmap: " + ex);
#endif
                return null;
            }
#if !DEBUG
            finally
            {
                GCSettings.LatencyMode = GCLatencyMode.Interactive;
            }
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
