using System;
using System.Globalization;
using System.Windows.Data;

namespace YouBoard.Views.Converters
{
    public class IssueDatesAndDurationsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
            {
                return Binding.DoNothing;
            }

            if (values[0] is not DateTime created)
            {
                return Binding.DoNothing;
            }

            var elapsed = values[1] is TimeSpan e ? e : TimeSpan.Zero;
            var estimated = values[2] is TimeSpan es ? es : TimeSpan.Zero;

            // Base: 作成 yy/MM/dd
            var createdPart = $"作成 {created:yy/MM/dd}";

            // Durations
            string elapsedPart = null;
            string estimatedPart = null;

            if (elapsed > TimeSpan.Zero)
            {
                // Round to the nearest minute like existing TotalMinutes behavior
                var minutes = (int)Math.Round(elapsed.TotalMinutes);
                elapsedPart = $"実績時間 {minutes} min";
            }

            if (estimated > TimeSpan.Zero)
            {
                var minutes = (int)Math.Round(estimated.TotalMinutes);
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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}