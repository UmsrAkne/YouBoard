using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace YouBoard.Views.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TimeSpan ts)
            {
                return Binding.DoNothing;
            }

            var mode = parameter as string ?? "Default";

            return mode switch
            {
                "TotalMinutes" => Math.Round(ts.TotalMinutes).ToString("0") + " 分",
                "TotalHours" => ts.TotalHours.ToString("0.0") + " 時間",
                "HoursMinutes" => $"{(int)ts.TotalHours}時間 {ts.Minutes}分",
                "Readable" => ToReadable(ts),
                _ => ts.ToString(@"hh\:mm\:ss"),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        private static string ToReadable(TimeSpan ts)
        {
            var parts = new[]
            {
                ts.Days > 0 ? $"{ts.Days}日" : null,
                ts.Hours > 0 ? $"{ts.Hours}時間" : null,
                ts.Minutes > 0 ? $"{ts.Minutes}分" : null,
                ts.Seconds > 0 ? $"{ts.Seconds}秒" : null,
            };

            return string.Join(" ", parts.Where(p => p != null));
        }
    }
}