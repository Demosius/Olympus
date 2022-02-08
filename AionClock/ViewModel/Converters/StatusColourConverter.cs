using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Uranus.Staff.Model;

namespace AionClock.ViewModel.Converters
{
    public class StatusColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EClockStatus val = (EClockStatus)Enum.Parse(typeof(EClockStatus), value?.ToString() ?? string.Empty);
            return val switch
            {
                EClockStatus.Approved => new(Colors.Green),
                EClockStatus.Rejected => new(Colors.Red),
                _ => new SolidColorBrush(Colors.Black)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
