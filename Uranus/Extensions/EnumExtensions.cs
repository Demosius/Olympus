using System;
using System.Linq;

namespace Uranus.Extensions;

public static class EnumExtensions
{
    public static string GetDescription<T>(this T e) where T : IConvertible
    {
        if (e is not Enum) return e.ToString() ?? string.Empty;

        var enumType = e.GetType();
        var memberInfo = enumType.GetMember(e.ToString() ?? string.Empty);

        if (memberInfo is not { Length: > 0 }) return e.ToString() ?? string.Empty;

        var attributes = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

        if (attributes.Any())
            return ((System.ComponentModel.DescriptionAttribute)attributes.ElementAt(0)).Description;

        return e.ToString() ?? string.Empty;
    }
}