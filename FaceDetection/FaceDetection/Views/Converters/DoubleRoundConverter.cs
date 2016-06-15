using System;
using System.Globalization;
using System.Windows.Data;

namespace FaceDetection.Views.Converters
{
    internal class DoubleRoundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double number = -1;

            try
            {
                number = Math.Round(System.Convert.ToDouble(value), 2);
            }
            catch (Exception)
            {
                // ignored
            }

            return number;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
