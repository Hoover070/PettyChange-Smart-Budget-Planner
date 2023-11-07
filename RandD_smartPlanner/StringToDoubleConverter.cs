using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace RandD_smartPlanner
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueStr = value.ToString();
            if (double.TryParse(valueStr, out double result))
                return result;

            return 0.0;  // default to 0.0 if the input is not a valid number
        }
    }
}