
namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static partial class Settings
        {
            internal static void init()
            {
                Video.init();
                Audio.init();
                Input.init();
                Network.init();
            }
            public static readonly string BUTTON_VIDEO
                = create("SETTINGS_TAB_BUTTON_VIDEO", "Display");
            public static readonly string BUTTON_AUDIO
                = create("SETTINGS_TAB_BUTTON_AUDIO", "Audio");
            public static readonly string BUTTON_INPUT
                = create("SETTINGS_TAB_BUTTON_INPUT", "Input");
            public static readonly string BUTTON_NETWORK
                = create("SETTINGS_TAB_BUTTON_NETWORK", "Interface");

            public static readonly string BUTTON_RESET_TO_DEFAULTS
                = create("SETTINGS_BUTTON_RESET_TO_DEFAULTS", "Reset to Defaults");
            public static readonly string BUTTON_RESET
                = create("SETTINGS_BUTTON_RESET", "Reset");
            public static readonly string BUTTON_CANCEL
                = create("SETTINGS_BUTTON_CANCEL", "Cancel");
            public static readonly string BUTTON_APPLY
                = create("SETTINGS_BUTTON_APPLY", "Apply");



            
            
        }

    }
}