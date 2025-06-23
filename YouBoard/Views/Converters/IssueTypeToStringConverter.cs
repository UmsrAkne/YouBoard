using System;
using System.Globalization;
using System.Windows.Data;
using YouBoard.Models;
using YouBoard.Utils;

namespace YouBoard.Views.Converters
{
    public class IssueTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IssueType issueType)
            {
                return IssueTypeHelper.ToIssueTypeName(issueType);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}