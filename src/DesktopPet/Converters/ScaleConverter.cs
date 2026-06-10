using System.Globalization;
using System.Windows.Data;

namespace DesktopPet.ViewModels;

/// <summary>
/// 缩放比例转换器
/// </summary>
public class ScaleConverter : IValueConverter
{
    public static readonly ScaleConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double scale)
            return scale;
        return 1.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
