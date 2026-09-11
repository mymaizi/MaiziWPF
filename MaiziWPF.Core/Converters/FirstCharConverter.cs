using System;
using System.Globalization;
using System.Windows.Data;

namespace MaiziWPF.Core
{
    public class FirstCharConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "A";
            string text = value.ToString();
            if (string.IsNullOrEmpty(text)) return "A";
            return text.Substring(0, 1).ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}