using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using RomForge.Core.Matching;

namespace RomForge.UI.Converters;

public sealed class MatchStatusToBrushConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value is MatchStatus status
            ? status switch
            {
                MatchStatus.Verified => StatusColors.Verified,
                MatchStatus.Missing => StatusColors.Missing,
                MatchStatus.IncorrectlyNamed => StatusColors.IncorrectlyNamed,
                MatchStatus.WrongArchiveType => StatusColors.WrongArchiveType,
                MatchStatus.Untrimmed => StatusColors.Untrimmed,
                _ => Brushes.Gray,
            }
            : null;

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
