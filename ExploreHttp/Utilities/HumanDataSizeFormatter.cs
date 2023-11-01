using System.Globalization;
using System.Windows.Data;

namespace ExploreHttp.Utilities;

[ValueConversion(typeof(long), typeof(string))]
public class HumanDataSizeFormatter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var v = (long)value;

        if (v > 1024 * 1024 * 1024)
        {
            var gb = v * 1.0 / (1024 * 1024 * 1024);
            return $"{gb:0.##}GB";
        }
        else if (v > 1024 * 1024)
        {
            var mb = v * 1.0 / (1024 * 1024);
            return $"{mb:0.##}MB";
        }
        else if (v > 1024)
        {
            var kb = v * 1.0 / 1024;
            return $"{kb:0.##}kB";
        }
        else
        {
            return $"{v}B";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
