using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Pantheon.ViewModels.Converter;

public class AsyncImageConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Uri uri) return null;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = uri;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();

        return bitmapImage;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
