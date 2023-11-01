using ExploreHttp.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ExploreHttp.Utilities;

[ValueConversion(typeof(RequestMethod), typeof(Brush))]
public class MethodToColorBrushConverter : IValueConverter
{
    public Brush IdempotentBrush { get; set; }
    public Brush NonIdempotentBrush { get; set; }
    public Brush DangerousBrush { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var method = RequestMethod.Get;
        if (value is RequestMethod)
            method = (RequestMethod)value;

        if (value is RequestMethod?)
            method = ((RequestMethod?)value).GetValueOrDefault();

        switch (method)
        {
            case RequestMethod.Get:
            case RequestMethod.Options:
            case RequestMethod.Head:
                return IdempotentBrush;
            case RequestMethod.Post:
            case RequestMethod.Put:
            case RequestMethod.Patch:
                return NonIdempotentBrush;
            case RequestMethod.Delete:
                return DangerousBrush;
            default:
                return DangerousBrush;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
