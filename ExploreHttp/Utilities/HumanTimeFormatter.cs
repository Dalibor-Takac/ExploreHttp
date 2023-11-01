using System.Globalization;
using System.Windows.Data;

namespace ExploreHttp.Utilities;

[ValueConversion(typeof(TimeSpan), typeof(string))]
public class HumanTimeFormatter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var ts = (TimeSpan)value;

        if (ts.TotalSeconds < 1)
            return $"{ts.TotalMilliseconds:0.##}ms";
        else if (ts.TotalSeconds < 60)
            return $"{ts.TotalSeconds:0.##}s";
        else if (ts.TotalMinutes < 60)
            return $"{ts.Minutes}m {ts.Seconds:0.##}s";
        else
            return ts.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
