using System;
using System.Globalization;
using System.Windows.Data;

namespace ControlesAccesoQR.Converters
{
    public class EqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            var x = values[0];
            var y = values[1];

            if (x == null || y == null)
                return x == y;

            // Allow string comparison without casing sensitivity
            if (x is string s1 && y is string s2)
                return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);

            return Equals(x, y);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
