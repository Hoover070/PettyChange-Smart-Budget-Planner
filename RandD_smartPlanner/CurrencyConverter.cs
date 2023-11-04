using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace RandD_smartPlanner
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
                return doubleValue.ToString("C", culture);

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueStr = value.ToString().Replace("$", "");
            if (double.TryParse(valueStr, out double result))
                return result;

            return 0.0;
        }
    }
}