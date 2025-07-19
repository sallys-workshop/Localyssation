
using System.ComponentModel;
using System;

namespace Localyssation
{
    public enum VanillaFonts
    {
        [Description("CENTAUR")]
        CENTAUR,
        [Description("terminal-grotesque")]
        TERMINAL_GROTESQUE,
        [Description("LiberationSans")]
        LIBRATION_SANS,
        [Description("HoboStd")]
        HOBOSTD,
        [Description("MSGOTHIC")]
        MSGOTHIC
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
