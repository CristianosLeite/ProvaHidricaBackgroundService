using System.Globalization;
using System.Windows.Data;
using ProvaHidrica.Models;

namespace ProvaHidrica.Converters
{
    public class VpOrCisConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Recipe recipe)
                return null;

            return string.IsNullOrEmpty(recipe.Vp)
                ? recipe.Cis?.ToString() ?? string.Empty
                : recipe.Vp;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
