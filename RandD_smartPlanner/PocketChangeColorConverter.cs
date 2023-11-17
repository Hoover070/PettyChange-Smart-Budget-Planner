using System;
using System.Globalization;
using Microsoft.Maui.Controls;





namespace RandD_smartPlanner
{
    public class PocketChangeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double pocketChange)
                return pocketChange >= 0 ? Colors.Green : Colors.Red;

            return Colors.Black; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}