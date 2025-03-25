using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CalculatorApp.Converters
{
    public class NumberBaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentBase && int.TryParse(parameter?.ToString(), out int parameterBase))
            {
                return currentBase == parameterBase;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && int.TryParse(parameter?.ToString(), out int parameterBase))
            {
                return parameterBase;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
