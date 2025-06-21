using System.Globalization;
using Incidenten.Domain.Enums;

namespace Incidenten.Mobile.Converters;

public class ShowCompleteConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (IncidentStatus)value == IncidentStatus.InProgress;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
