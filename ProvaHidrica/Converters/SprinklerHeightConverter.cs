using System.Globalization;
using System.Windows.Data;

namespace ProvaHidrica.Converters
{
    public class SprinklerHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int height)
            {
                return height switch
                {
                    1 => "Alto",
                    2 => "Médio",
                    3 => "Baixo",
                    _ => "Desconhecido",
                };
            }
            return "Desconhecido";
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            if (value is string height)
            {
                return height switch
                {
                    "Alto" => 1,
                    "Médio" => 2,
                    "Baixo" => 3,
                    _ => 0,
                };
            }
            return 0;
        }
    }
}
