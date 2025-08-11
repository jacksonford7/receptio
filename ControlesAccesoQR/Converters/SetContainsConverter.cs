using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using ControlesAccesoQR.Estados;

namespace ControlesAccesoQR.Converters
{
    public class SetContainsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            if (values[0] is ISet<EstadoProceso> set && values[1] is EstadoProceso estado)
                return set.Contains(estado);

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
