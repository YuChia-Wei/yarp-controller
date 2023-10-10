using System.ComponentModel;

namespace Yarp.ControlPlant.Infra.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        if (field == null)
        {
            return value.ToString();
        }

        var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))!;

        return attr.Description;
    }
}