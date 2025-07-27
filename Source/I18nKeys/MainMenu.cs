
namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static class MainMenu
        {
            internal static void Init() { }
            public static readonly string BUTTON_SINGLEPLAY
                = Create("MAIN_MENU_BUTTON_SINGLEPLAY", "Singleplayer");
            public static readonly string BUTTON_SINGLEPLAY_TOOLTIP
                = Create("MAIN_MENU_BUTTON_SINGLEPLAY_TOOLTIP", "Start a Singleplayer Game.");

            public static readonly string BUTTON_MULTIPLAY
                = Create("MAIN_MENU_BUTTON_MULTIPLAY", "Multiplayer");
            public static readonly string BUTTON_MULTIPLAY_TOOLTIP
                = Create("MAIN_MENU_BUTTON_MULTIPLAY_TOOLTIP", "Start a Netplay Game.");
            public static readonly string BUTTON_MULTIPLAY_DISABLED_TOOLTIP
                = Create("MAIN_MENU_BUTTON_MULTIPLAY_DISABLED_TOOLTIP", "Multiplayer is disabled on this demo.");

            public static readonly string BUTTON_SETTINGS
                = Create("MAIN_MENU_BUTTON_SETTINGS", "Settings");
            public static readonly string BUTTON_SETTINGS_TOOLTIP
                = Create("MAIN_MENU_BUTTON_SETTINGS_TOOLTIP", "Configure Game Settings.");

            public static readonly string BUTTON_QUIT
                = Create("MAIN_MENU_BUTTON_QUIT", "Quit");
            public static readonly string BUTTON_QUIT_TOOLTIP
                = Create("MAIN_MENU_BUTTON_QUIT_TOOLTIP", "End The Application.");

            public static readonly string PAGER
                = Create("MAIN_MENU_PAGER", "Page ( {0} / 15 )");

            public static readonly string BUTTON_JOIN_SERVER
                = Create("MAIN_MENU_" + nameof(BUTTON_JOIN_SERVER), "Join");
            public static readonly string BUTTON_HOST_SERVER
                = Create("MAIN_MENU_" + nameof(BUTTON_HOST_SERVER), "Host");
            public static readonly string BUTTON_RETURN
                = Create("MAIN_MENU_" + nameof(BUTTON_RETURN), "Return");
        }

    }
}