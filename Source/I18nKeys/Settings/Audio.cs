﻿
namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static partial class Settings
        {
            internal static class Audio
            {
                internal static void Init() { }
                public static readonly string HEADER_AUDIO_SETTINGS
                    = Create("SETTINGS_AUDIO_HEADER_AUDIO_SETTINGS", "Audio Settings");
                public static readonly string CELL_MASTER_VOLUME
                    = Create("SETTINGS_AUDIO_CELL_MASTER_VOLUME", "Master Volume");
                public static readonly string CELL_MUTE_APPLICATION
                    = Create("SETTINGS_AUDIO_CELL_MUTE_APPLICATION", "Mute Application");
                public static readonly string CELL_MUTE_MUSIC
                    = Create("SETTINGS_AUDIO_CELL_MUTE_MUSIC", "Mute Music");

                public static readonly string HEADER_AUDIO_CHANNEL_SETTINGS
                    = Create("SETTINGS_AUDIO_HEADER_AUDIO_CHANNEL_SETTINGS", "Audio Channels");
                public static readonly string CELL_GAME_VOLUME
                    = Create("SETTINGS_AUDIO_CELL_GAME_VOLUME", "Game Volume");
                public static readonly string CELL_GUI_VOLUME
                    = Create("SETTINGS_AUDIO_CELL_GUI_VOLUME", "GUI Volume");
                public static readonly string CELL_AMBIENCE_VOLUME
                    = Create("SETTINGS_AUDIO_CELL_AMBIENCE_VOLUME", "Ambience Volume");
                public static readonly string CELL_MUSIC_VOLUME
                    = Create("SETTINGS_AUDIO_CELL_MUSIC_VOLUME", "Music Volume");
                public static readonly string CELL_VOICE_VOLUME
                    = Create("SETTINGS_AUDIO_CELL_VOICE_VOLUME", "Voice Volume");
            }

        }
    }
}