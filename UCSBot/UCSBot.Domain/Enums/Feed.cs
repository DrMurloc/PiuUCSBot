using System.ComponentModel;
using System.Reflection;

namespace UCSBot.Domain.Enums;

public enum Feed
{
    [Description("Pump It Up UCS Feed")] Ucs
}

public static class FeedHelper
{
    public static string GetDescription(this Feed enumValue)
    {
        return enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttribute<DescriptionAttribute>()
            ?.Description ?? "Unknown Feed";
    }
}