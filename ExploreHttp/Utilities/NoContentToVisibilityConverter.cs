using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ExploreHttp.Utilities;

[ValueConversion(typeof(object), typeof(Visibility))]
public class NoContentToVisibilityConverter : IValueConverter
{
    public Visibility DefaultOrSimilarMap { get; set; }
    public Visibility NonDefaultOrSimilarMap { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return DefaultOrSimilarMap;

        var valueType = value.GetType();

        // unwrap nullable type into base one
        if (valueType.IsGenericType && valueType.IsAssignableFrom(typeof(Nullable<>)))
            valueType = valueType.GenericTypeArguments[0];

        if (valueType.IsValueType)
        {
            var defaultValue = Activator.CreateInstance(valueType);
            if (value == defaultValue)
                return DefaultOrSimilarMap;
        }

        if (value is string v)
        {
            if (string.IsNullOrEmpty(v))
                return DefaultOrSimilarMap;
        }

        return NonDefaultOrSimilarMap;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
