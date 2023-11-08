using ExploreHttp.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ExploreHttp.Utilities;
[ValueConversion(typeof(LogLevel), typeof(Brush))]
public class LogLevelToBrushConverter : IValueConverter
{
    public Brush DebugBrush { get; set; }
    public Brush InfoBrush { get; set; }
    public Brush WarningBrush { get; set; }
    public Brush ErrorBrush { get; set; }
    public Brush FatalBrush { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var level = (LogLevel)value;
        return level switch
        {
            LogLevel.Fatal => FatalBrush,
            LogLevel.Error => ErrorBrush,
            LogLevel.Warning => WarningBrush,
            LogLevel.Info => InfoBrush,
            LogLevel.Debug => DebugBrush,
            _ => InfoBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
