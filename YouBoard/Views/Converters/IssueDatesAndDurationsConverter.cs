using System;
using System.Globalization;
using System.Windows.Data;
using YouBoard.Models;

namespace YouBoard.Views.Converters
{
    public class IssueDatesAndDurationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IssueWrapper issue)
            {
                return Binding.DoNothing;
            }

            // Base: 作成 yy/MM/dd
            var createdPart = $"作成 {issue.Created:yy/MM/dd}";

            // Durations
            string elapsedPart = null;
            string estimatedPart = null;

            if (issue.ElapsedDuration > TimeSpan.Zero)
            {
                // Round to the nearest minute like existing TotalMinutes behavior
                var minutes = (int)Math.Round(issue.ElapsedDuration.TotalMinutes);
                elapsedPart = $"実績時間 {minutes} min";
            }

            if (issue.EstimatedDuration > TimeSpan.Zero)
            {
                var minutes = (int)Math.Round(issue.EstimatedDuration.TotalMinutes);
                estimatedPart = $"見積もり {minutes} min";
            }

            // Compose with separators "/" only for existing parts
            if (elapsedPart == null && estimatedPart == null)
            {
                return createdPart;
            }

            if (elapsedPart != null && estimatedPart != null)
            {
                return $"{createdPart} / {elapsedPart} / {estimatedPart}";
            }

            return $"{createdPart} / {elapsedPart ?? estimatedPart}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}