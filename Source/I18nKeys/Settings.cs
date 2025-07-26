
namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static partial class Settings
        {
            internal static void Init()
            {
                Video.Init();
                Audio.Init();
                Input.init();
                Network.Init();
                Mod.Init();
            }
            public static readonly string BUTTON_VIDEO
                = Create("SETTINGS_TAB_BUTTON_VIDEO", "Display");
            public static readonly string BUTTON_AUDIO
                = Create("SETTINGS_TAB_BUTTON_AUDIO", "Audio");
            public static readonly string BUTTON_INPUT
                = Create("SETTINGS_TAB_BUTTON_INPUT", "Input");
            public static readonly string BUTTON_NETWORK
                = Create("SETTINGS_TAB_BUTTON_NETWORK", "Interface");
            public static readonly string BUTTON_MODS
                = Create("SETTINGS_TAB_BUTTON_MODS", "Mods");

            public static readonly string BUTTON_RESET_TO_DEFAULTS
                = Create("SETTINGS_BUTTON_RESET_TO_DEFAULTS", "Reset to Defaults");
            public static readonly string BUTTON_RESET
                = Create("SETTINGS_BUTTON_RESET", "Reset");
            public static readonly string BUTTON_CANCEL
                = Create("SETTINGS_BUTTON_CANCEL", "Cancel");
            public static readonly string BUTTON_APPLY
                = Create("SETTINGS_BUTTON_APPLY", "Apply");





        }

    }
}