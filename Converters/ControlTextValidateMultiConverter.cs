using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    public class MultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool returned = false;
            foreach (var item in values)
            {
                string text = item.ToString();
                returned = !string.IsNullOrWhiteSpace(text);
                if (string.IsNullOrWhiteSpace(text))
                {
                    returned = false;
                    break;
                }
            }
            return returned;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
