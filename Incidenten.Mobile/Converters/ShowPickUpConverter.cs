using System.Globalization;
using Incidenten.Domain.Enums;

namespace Incidenten.Mobile.Converters;

public class ShowPickUpConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (IncidentStatus)value == IncidentStatus.Registered;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}