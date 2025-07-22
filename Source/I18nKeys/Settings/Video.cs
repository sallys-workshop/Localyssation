
using System;
using System.Linq;

namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static partial class Settings
        {
            internal static class Video
            {
                private static string CreateCell(string key, string defaultString) => Create($"SETTINGS_VIDEO_CELL_{key}", defaultString);

                private static string[] CreateOptions(string parentKey, string[] defaultStrings)
                {
                    return defaultStrings.Select((value, index) => Create($"{parentKey}_OPTION_{index + 1}", value))
                    .ToArray();
                }
                internal static void Init() { }
                public static readonly string HEADER_GAME_EFFECT_SETTINGS
                    = Create("SETTINGS_VIDEO_HEADER_GAME_EFFECT_SETTINGS", "Display Sensitive Settings");
                public static readonly string CELL_PROPORTIONS_TOGGLE
                    = Create("SETTINGS_VIDEO_CELL_PROPORTIONS_TOGGLE", "Limit Player Character Proportions");
                public static readonly string CELL_JIGGLE_BONES_TOGGLE
                    = Create("SETTINGS_VIDEO_CELL_JIGGLE_BONES_TOGGLE", "Disable Suggestive Jiggle Bones");
                public static readonly string CELL_CLEAR_UNDERCLOTHES_TOGGLE
                    = Create("SETTINGS_VIDEO_CELL_CLEAR_UNDERCLOTHES_TOGGLE", "Enable Clear Clothing");

                public static readonly string HEADER_VIDEO_SETTINGS
                    = Create("SETTINGS_VIDEO_HEADER_VIDEO_SETTINGS", "Video Settings");
                public static readonly string CELL_SCREEN_MODE
                    = CreateCell("SCREEN_MODE", "Screen Mode");
                public static readonly string[] CELL_SCREEN_MODE_OPTIONS
                    = CreateOptions(CELL_SCREEN_MODE, new string[] { 
                        "Windowed", 
                        "Fullscreen", 
                        "Fullscreen (Borderless)" 
                    });
                public static readonly string CELL_FULLSCREEN_TOGGLE
                    = Create("SETTINGS_VIDEO_CELL_FULLSCREEN_TOGGLE", "Fullscreen Mode");
                public static readonly string CELL_VERTICAL_SYNC
                    = Create("SETTINGS_VIDEO_CELL_VERTICAL_SYNC", "Vertical Sync / Lock 60 FPS");
                public static readonly string CELL_ANISOTROPIC_FILTERING
                    = Create("SETTINGS_VIDEO_CELL_ANISOTROPIC_FILTERING", "Anisotropic Filtering");
                public static readonly string CELL_SCREEN_RESOLUTION
                    = Create("SETTINGS_VIDEO_CELL_SCREEN_RESOLUTION", "Screen Resolution");
                public static readonly string CELL_ANTI_ALIASING
                    = Create("SETTINGS_VIDEO_CELL_ANTI_ALIASING", "Anti Aliasing");
                public static readonly string[] CELL_ANTI_ALIASING_OPTIONS
                    = CreateOptions(CELL_ANTI_ALIASING, new string[] {
                        "Disabled",
                        "2x Multi Sampling",
                        "4x Multi Sampling",
                        "8x Multi Sampling"
                    });

                public static readonly string CELL_TEXTURE_FILTERING
                    = Create("SETTINGS_VIDEO_CELL_TEXTURE_FILTERING", "Texture Filtering");
                public static readonly string[] CELL_TEXTURE_FILTERING_OPTIONS
                    = CreateOptions(CELL_TEXTURE_FILTERING, new string[] {
                        "Bilnear (Smooth)",
                        "Nearest (Crunchy)"
                    });
                //public static readonly string CELL_TEXTURE_FILTERING_OPTION_1
                //    = Create("SETTINGS_VIDEO_CELL_TEXTURE_FILTERING_OPTION_1", "Bilnear (Smooth)");
                //public static readonly string CELL_TEXTURE_FILTERING_OPTION_2
                //    = Create("SETTINGS_VIDEO_CELL_TEXTURE_FILTERING_OPTION_2", "Nearest (Crunchy)");
                public static readonly string CELL_TEXTURE_QUALITY
                    = Create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY", "Texture Quality");
                public static readonly string[] CELL_TEXTURE_QUALITY_OPTIONS
                    = CreateOptions(CELL_TEXTURE_QUALITY, new string[] {
                        "High",
                        "Medium",
                        "Low",
                        "Very Low"
                    });
                //public static readonly string CELL_TEXTURE_QUALITY_OPTION_1
                //    = Create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_1", "High");
                //public static readonly string CELL_TEXTURE_QUALITY_OPTION_2
                //    = Create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_2", "Medium");
                //public static readonly string CELL_TEXTURE_QUALITY_OPTION_3
                //    = Create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_3", "Low");
                //public static readonly string CELL_TEXTURE_QUALITY_OPTION_4
                //    = Create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_4", "Very Low");

                public static readonly string HEADER_CAMERA_SETTINGS
                    = Create("SETTINGS_VIDEO_HEADER_CAMERA_SETTINGS", "Camera Display Settings");
                public static readonly string CELL_FIELD_OF_VIEW
                    = Create("SETTINGS_VIDEO_CELL_FIELD_OF_VIEW", "Field Of View");
                public static readonly string CELL_CAMERA_SMOOTHING
                    = Create("SETTINGS_VIDEO_CELL_CAMERA_SMOOTHING", "Camera Smoothing");
                public static readonly string CELL_CAMERA_HORIZ
                    = Create("SETTINGS_VIDEO_CELL_CAMERA_HORIZ", "Camera X Position");
                public static readonly string CELL_CAMERA_VERT
                    = Create("SETTINGS_VIDEO_CELL_CAMERA_VERT", "Camera Y Position");
                public static readonly string CELL_CAMERA_RENDER_DISTANCE
                    = Create("SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE", "Render Distance");
                public static readonly string[] CELL_CAMERA_RENDER_DISTANCE_OPTIONS
                    = CreateOptions(CELL_CAMERA_RENDER_DISTANCE, new string[] {
                        "Very Near",
                        "Near",
                        "Far",
                        "Very Far"
                    });
                
                public static readonly string HEADER_CURSOR_SETTINGS
                    = Create("SETTINGS_VIDEO_HEADER_CURSOR_SETTINGS", "Cursor Settings");
                public static readonly string CELL_CURSOR_GRAPHIC
                    = Create("SETTINGS_VIDEO_CELL_CURSOR_GRAPHIC", "Cursor Graphic");
                public static readonly string CELL_HARDWARE_CURSOR
                    = Create("SETTINGS_VIDEO_CELL_HARDWARE_CURSOR", "Hardware Cursor");

                public static readonly string HEADER_POST_PROCESSING
                    = Create("SETTINGS_VIDEO_HEADER_POST_PROCESSING", "Post Processing");
                public static readonly string CELL_CAMERA_BITCRUSH_SHADER
                    = Create("SETTINGS_VIDEO_CELL_CAMERA_BITCRUSH_SHADER", "Enable Bitcrush Shader");
                public static readonly string CELL_CAMERA_WATER_EFFECT
                    = Create("SETTINGS_VIDEO_CELL_CAMERA_WATER_EFFECT", "Enable Underwater Distortion Shader");
                public static readonly string CELL_CAMERA_SHAKE
                    = Create("SETTINGS_VIDEO_CELL_CAMERA_SHAKE", "Enable Screen Shake");
                public static readonly string CELL_WEAPON_GLOW
                    = Create("SETTINGS_VIDEO_CELL_WEAPON_GLOW", "Disable Weapon Glow Effect");
                public static readonly string CELL_DISABLE_GIB_EFFECT
                    = Create("SETTINGS_VIDEO_CELL_DISABLE_GIB_EFFECT", "Disable Gib Effect");
            }

        }
    }
}