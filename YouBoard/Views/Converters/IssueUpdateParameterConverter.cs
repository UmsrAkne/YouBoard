using System;
using System.Globalization;
using System.Windows.Data;
using YouBoard.Models;

namespace YouBoard.Views.Converters
{
    public sealed class IssueUpdateParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new IssueUpdateParameter
            {
                IssueWrapper = (IssueWrapper)values[0],
                UpdatePropertyName = (string)values[1],
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}