using System;
using Windows.UI.Xaml.Data;

namespace Console.Convertidores
{
    internal class Entero : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? "" : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Int64.TryParse(value as string, out long resultado);
            return resultado;
        }
    }
}
