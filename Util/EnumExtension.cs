using System.Reflection;

namespace Google.Drive.Query.Integration.Util
{
    public static class EnumExtension
    {
        public static string? GetStringValue(this Enum value)
        {
            FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());

            StringValueAttribute[]? attributes = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attributes.Length > 0 ? attributes[0].StringValue : null;
        }
    }
}
