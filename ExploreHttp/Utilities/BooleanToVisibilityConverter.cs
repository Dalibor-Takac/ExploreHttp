using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ExploreHttp.Utilities
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueMapping { get; set; }
        public Visibility FalseMapping { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool)
                flag = (bool)value;

            if (value is bool?)
                flag = ((bool?)value).GetValueOrDefault();

            return flag ? TrueMapping : FalseMapping;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
