using System;
using System.ComponentModel;
using UnityEngine;

namespace Localyssation.Util
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
    public static class PathUtil
    {
        public static string GetChildTransformPath(Transform transform, int depth = 0)
        {
            var str = transform.name;
            if (depth > 0)
            {
                var parent = transform.parent;
                if (parent != null)
                {
                    str = $"{GetChildTransformPath(parent, depth - 1)}/{str}";
                }
            }
            return str;
        }
        public static string GetPath(Transform transform)
        {
            string path = transform.name;
            Transform current = transform;

            // 从当前节点向上遍历到根节点
            while (current.parent != null)
            {
                current = current.parent;
                path = current.name + "/" + path; // 从根向子节点拼接
            }

            return "/" + path; // 添加根路径斜杠
        }
    }

}
