using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SubspaceStats.Models
{
    public static class EnumExtensions
    {
        public static string ToDisplayString(this Enum enumValue)
        {
            return enumValue
                .GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.GetName() ?? enumValue.ToString();
        }
    }
}
