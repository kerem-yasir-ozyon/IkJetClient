using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace IkJetApp.Helpers
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var enumMember = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault();

            var displayAttribute = enumMember?.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.GetName() ?? enumValue.ToString();
        }
    }
}
