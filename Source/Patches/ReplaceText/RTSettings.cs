using HarmonyLib;
using Localyssation.LangAdjutable;
using Localyssation.LanguageModule;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        // settings
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Start))]
        [HarmonyPostfix]
        public static void SettingsManager_Start(SettingsManager __instance)
        {
            RTUtil.RemapAllTextUnderObject(__instance.gameObject, new Dictionary<string, string>()
            {
                //{ "Button_videoTab", "SETTINGS_TAB_BUTTON_VIDEO" },
                { "Button_videoTab", I18nKeys.Settings.BUTTON_VIDEO },

                { "_header_GameEffectSettings", I18nKeys.Settings.Video.HEADER_GAME_EFFECT_SETTINGS },
                //{ "_cell_proportionsToggle", I18nKeys.Settings.Video.CELL_PROPORTIONS_TOGGLE },
                { "_cell_jiggleBonesToggle", I18nKeys.Settings.Video.CELL_JIGGLE_BONES_TOGGLE },
                { "_cell_clearUnderclothesToggle", I18nKeys.Settings.Video.CELL_CLEAR_UNDERCLOTHES_TOGGLE },
                { "_cell_screenMode", I18nKeys.Settings.Video.CELL_SCREEN_MODE },

                { "_header_videoSettings", I18nKeys.Settings.Video.HEADER_VIDEO_SETTINGS },
                //{ "_cell_fullscreenToggle", I18nKeys.Settings.Video.CELL_FULLSCREEN_TOGGLE },
                { "_cell_verticalSync", I18nKeys.Settings.Video.CELL_VERTICAL_SYNC },
                { "_cell_anisotropicFiltering", I18nKeys.Settings.Video.CELL_ANISOTROPIC_FILTERING },
                { "_cell_screenResolution", I18nKeys.Settings.Video.CELL_SCREEN_RESOLUTION },
                { "_cell_antiAliasing", I18nKeys.Settings.Video.CELL_ANTI_ALIASING },
                { "_cell_textureFiltering", I18nKeys.Settings.Video.CELL_TEXTURE_FILTERING },
                { "_cell_textureQuality", I18nKeys.Settings.Video.CELL_TEXTURE_QUALITY },

                { "_header_CameraSettings", I18nKeys.Settings.Video.HEADER_CAMERA_SETTINGS },
                { "_cell_fieldOfView", I18nKeys.Settings.Video.CELL_FIELD_OF_VIEW },
                { "_cell_cameraSmoothing", I18nKeys.Settings.Video.CELL_CAMERA_SMOOTHING },
                { "_cell_cameraHoriz", I18nKeys.Settings.Video.CELL_CAMERA_HORIZ },
                { "_cell_cameraVert", I18nKeys.Settings.Video.CELL_CAMERA_VERT },
                { "_cell_cameraRenderDistance", I18nKeys.Settings.Video.CELL_CAMERA_RENDER_DISTANCE },

                { "_header_cursorSettings", I18nKeys.Settings.Video.HEADER_CURSOR_SETTINGS },
                { "_cell_setCursor", I18nKeys.Settings.Video.CELL_CURSOR_GRAPHIC },
                { "_cell_useHardwareCursor", I18nKeys.Settings.Video.CELL_HARDWARE_CURSOR },

                { "_header_PostProcessing", I18nKeys.Settings.Video.HEADER_POST_PROCESSING },
                { "_cell_cameraBitcrushShader", I18nKeys.Settings.Video.CELL_CAMERA_BITCRUSH_SHADER },
                { "_cell_cameraWaterEffect", I18nKeys.Settings.Video.CELL_CAMERA_WATER_EFFECT },
                { "_cell_cameraShake", I18nKeys.Settings.Video.CELL_CAMERA_SHAKE },
                { "_cell_weaponGlow", I18nKeys.Settings.Video.CELL_WEAPON_GLOW },
                { "_cell_disableGibs", I18nKeys.Settings.Video.CELL_DISABLE_GIB_EFFECT },


                { "Button_audioTab", I18nKeys.Settings.BUTTON_AUDIO },

                { "_header_audioSettings", I18nKeys.Settings.Audio.HEADER_AUDIO_SETTINGS },
                { "_cell_masterVolume", I18nKeys.Settings.Audio.CELL_MASTER_VOLUME },
                { "_cell_muteApplication", I18nKeys.Settings.Audio.CELL_MUTE_APPLICATION },
                { "_cell_muteMusic", I18nKeys.Settings.Audio.CELL_MUTE_MUSIC },

                { "_header_audioChannelSettings_01", I18nKeys.Settings.Audio.HEADER_AUDIO_CHANNEL_SETTINGS },
                { "_cell_gameVolume", I18nKeys.Settings.Audio.CELL_GAME_VOLUME },
                { "_cell_guiVolume", I18nKeys.Settings.Audio.CELL_GUI_VOLUME },
                { "_cell_ambienceVolume", I18nKeys.Settings.Audio.CELL_AMBIENCE_VOLUME },
                { "_cell_musicVolume", I18nKeys.Settings.Audio.CELL_MUSIC_VOLUME },
                { "_cell_voiceVolume", I18nKeys.Settings.Audio.CELL_VOICE_VOLUME },


                { "Button_inputTab", I18nKeys.Settings.BUTTON_INPUT },

                { "Image_05", I18nKeys.Settings.Input.HEADER_INPUT_SETTINGS },
                { "_cell_axisType", I18nKeys.Settings.Input.CELL_AXIS_TYPE },
                { "Image_06", I18nKeys.Settings.Input.GAME_PAD_WIP },
                { "Image_07", I18nKeys.Settings.Input.CELL_RESET_BINDINGS },
                { "InputDefaults_button", I18nKeys.Settings.Input.CELL_RESET_BINDINGS },

                { "Image_08", I18nKeys.Settings.Input.HEADER_CAMERA_CONTROL },
                { "_cell_cameraSensitivity", I18nKeys.Settings.Input.CELL_CAMERA_SENSITIVITY },
                { "_cell_invertXCameraAxis", I18nKeys.Settings.Input.CELL_INVERT_X_CAMERA_AXIS },
                { "_cell_invertYCameraAxis", I18nKeys.Settings.Input.CELL_INVERT_Y_CAMERA_AXIS },
                { "_cell_keybinding_37", I18nKeys.Settings.Input.CELL_KEYBINDING_RESET_CAMERA },

                { "Header_Movement", I18nKeys.Settings.Input.HEADER_MOVEMENT },
                { "_cell_keybinding_up", I18nKeys.Settings.Input.CELL_KEYBINDING_UP },
                { "_cell_keybinding_down", I18nKeys.Settings.Input.CELL_KEYBINDING_DOWN },
                { "_cell_keybinding_left", I18nKeys.Settings.Input.CELL_KEYBINDING_LEFT },
                { "_cell_keybinding_right", I18nKeys.Settings.Input.CELL_KEYBINDING_RIGHT },
                { "_cell_keybinding_jump", I18nKeys.Settings.Input.CELL_KEYBINDING_JUMP },
                { "_cell_keybinding_dash", I18nKeys.Settings.Input.CELL_KEYBINDING_DASH },

                { "Header_Strafing", I18nKeys.Settings.Input.HEADER_STRAFING },
                { "_cell_keybinding_lockDirection", I18nKeys.Settings.Input.CELL_KEYBINDING_LOCK_DIRECTION },
                { "_cell_strafeMode", I18nKeys.Settings.Input.CELL_KEYBINDING_STRAFE_MODE },
                { "_cell_strafeWeapon", I18nKeys.Settings.Input.CELL_KEYBINDING_STRAFE_WEAPON },
                { "_cell_strafeCasting", I18nKeys.Settings.Input.CELL_KEYBINDING_STRAFE_CASTING },

                { "Header_Action", I18nKeys.Settings.Input.HEADER_ACTION },
                { "_cell_keybinding_attack", I18nKeys.Settings.Input.CELL_KEYBINDING_ATTACK },
                { "_cell_keybinding_chargeAttack", I18nKeys.Settings.Input.CELL_KEYBINDING_CHARGE_ATTACK },
                { "_cell_keybinding_block", I18nKeys.Settings.Input.CELL_KEYBINDING_BLOCK },
                { "_cell_keybinding_target", I18nKeys.Settings.Input.CELL_KEYBINDING_TARGET },
                { "_cell_keybinding_interact", I18nKeys.Settings.Input.CELL_KEYBINDING_INTERACT },
                { "_cell_keybinding_pvpFlag", I18nKeys.Settings.Input.CELL_KEYBINDING_PVP_FLAG },
                { "_cell_keybinding_skillSlot01", I18nKeys.Settings.Input.CELL_KEYBINDING_SKILL_SLOT_01 },
                { "_cell_keybinding_skillSlot02", I18nKeys.Settings.Input.CELL_KEYBINDING_SKILL_SLOT_02 },
                { "_cell_keybinding_skillSlot03", I18nKeys.Settings.Input.CELL_KEYBINDING_SKILL_SLOT_03 },
                { "_cell_keybinding_skillSlot04", I18nKeys.Settings.Input.CELL_KEYBINDING_SKILL_SLOT_04 },
                { "_cell_keybinding_skillSlot05", I18nKeys.Settings.Input.CELL_KEYBINDING_SKILL_SLOT_05 },
                { "_cell_keybinding_skillSlot06", I18nKeys.Settings.Input.CELL_KEYBINDING_SKILL_SLOT_06 },
                { "_cell_keybinding_recall", I18nKeys.Settings.Input.CELL_KEYBINDING_RECALL },
                { "_cell_keybinding_quickswapWeapon", I18nKeys.Settings.Input.CELL_KEYBINDING_QUICKSWAP_WEAPON },
                { "_cell_keybinding_sheatheWeapon", I18nKeys.Settings.Input.CELL_KEYBINDING_SHEATHE_WEAPON },
                { "_cell_keybinding_sit", I18nKeys.Settings.Input.CELL_KEYBINDING_SIT },

                { "Header_ConsumableSlots", I18nKeys.Settings.Input.HEADER_CONSUMABLE_SLOTS },
                { "_cell_keybinding_quickSlot01", I18nKeys.Settings.Input.CELL_KEYBINDING_QUICK_SLOT_01 },
                { "_cell_keybinding_quickSlot02", I18nKeys.Settings.Input.CELL_KEYBINDING_QUICK_SLOT_02 },
                { "_cell_keybinding_quickSlot03", I18nKeys.Settings.Input.CELL_KEYBINDING_QUICK_SLOT_03 },
                { "_cell_keybinding_quickSlot04", I18nKeys.Settings.Input.CELL_KEYBINDING_QUICK_SLOT_04 },
                //{ "_cell_keybinding_quickSlot05", I18nKeys.Settings.Input.CELL_KEYBINDING_QUICK_SLOT_05 },

                { "Header_Interface", I18nKeys.Settings.Input.HEADER_INTERFACE },
                { "_cell_keybinding_38", I18nKeys.Settings.Input.CELL_KEYBINDING_HOST_CONSOLE },
                { "_cell_keybinding_lexicon", I18nKeys.Settings.Input.CELL_KEYBINDING_LEXICON },
                { "_cell_keybinding_tabMenu", I18nKeys.Settings.Input.CELL_KEYBINDING_TAB_MENU },
                { "_cell_keybinding_statsTab", I18nKeys.Settings.Input.CELL_KEYBINDING_STATS_TAB },
                { "_cell_keybinding_skillsTab", I18nKeys.Settings.Input.CELL_KEYBINDING_SKILLS_TAB },
                { "_cell_keybinding_itemTab", I18nKeys.Settings.Input.CELL_KEYBINDING_ITEM_TAB },
                { "_cell_keybinding_questTab", I18nKeys.Settings.Input.CELL_KEYBINDING_QUEST_TAB },
                { "_cell_keybinding_whoTab", I18nKeys.Settings.Input.CELL_KEYBINDING_WHO_TAB },
                { "_cell_keybinding_hideUI", I18nKeys.Settings.Input.CELL_KEYBINDING_HIDE_UI },


                { "Button_gameTab", I18nKeys.Settings.BUTTON_NETWORK },

                { "_header_gameSettings", I18nKeys.Settings.Network.HEADER_GAME_SETTINGS },
                { "_cell_enablePvPOnMapEnter", I18nKeys.Settings.Network.CELL_ENABLE_PVP_ON_MAP_ENTER },

                { "_header_nametagSettings", I18nKeys.Settings.Network.HEADER_NAMETAG_SETTINGS },
                { "_cell_displayGlobalNicknameTags", I18nKeys.Settings.Network.CELL_DISPLAY_GLOBAL_NICKNAME_TAGS },
                { "_cell_displayLocalNametag", I18nKeys.Settings.Network.CELL_DISPLAY_LOCAL_NAMETAG },
                { "_cell_displayHostTag", I18nKeys.Settings.Network.CELL_DISPLAY_HOST_TAG },

                { "_header_uiSettings", I18nKeys.Settings.Network.HEADER_UI_SETTINGS },
                //{ "_cell_displayCreepNametags", I18nKeys.Settings.Network.CELL_DISPLAY_CREEP_NAMETAGS },
                //{ "_cell_hideDungeonMinimap", I18nKeys.Settings.Network.CELL_HIDE_DUNGEON_MINIMAP },
                { "_cell_hideFPSCounter", I18nKeys.Settings.Network.CELL_HIDE_FPS_COUNTER },
                { "_cell_hidePingCounter", I18nKeys.Settings.Network.CELL_HIDE_PING_COUNTER },
                { "_cell_hideStatPointCounter", I18nKeys.Settings.Network.CELL_HIDE_STAT_POINT_COUNTER },
                { "_cell_hideSkillPointCounter", I18nKeys.Settings.Network.CELL_HIDE_SKILL_POINT_COUNTER },
                { "_cell_hideQuestTracker", I18nKeys.Settings.Network.CELL_HIDE_QUEST_TRACKER },
                { "_cell_hideMinimap", I18nKeys.Settings.Network.CELL_HIDE_MINIMAP },
                { "_cell_hideDamageIcons", I18nKeys.Settings.Network.CELL_HIDE_DAMAGE_VALUE_NUMBER_ICONS },

                { "_header_chatboxSettings", I18nKeys.Settings.Network.HEADER_CHATBOX_SETTINGS },
                { "_cell_defaultChatRoom", I18nKeys.Settings.Network.CELL_DEFAULT_CHANNEL},
                { "_cell_fadeChatText", I18nKeys.Settings.Network.CELL_FADE_CHAT_TEXT },
                { "_cell_fadeGameFeed", I18nKeys.Settings.Network.CELL_FADE_GAME_FEED_TEXT },

                //{ "_header_clientSettings", I18nKeys.Settings.Network.HEADER_CLIENT_SETTINGS },


                { "Button_cancelSettings", I18nKeys.Settings.BUTTON_CANCEL },
                { "Button_applySettings", I18nKeys.Settings.BUTTON_APPLY },
            }, (textParent, key) =>
            {
                var dropdown = textParent.GetComponentInChildren<Dropdown>();
                if (dropdown)
                {
                    var dropdownOptionsTextFuncs = new List<Func<int, string>>();
                    for (var i = 0; i < dropdown.options.Count; i++)
                    {
                        var option = dropdown.options[i];
                        var dropdownOptionKey = $"{key}_OPTION_{i + 1}";
                        if (LanguageManager.DefaultLanguage.TryGetString(dropdownOptionKey, out var dropdownOptionText))
                        {
                            dropdownOptionsTextFuncs.Add(LangAdjustables.GetStringFunc(dropdownOptionKey, option.text));
                        }
                        else
                        {
                            //Localyssation.logger.LogWarning($"Cannot find translation key `{dropdownOptionKey}` for dropdown `{dropdown.name}`");
                        }
                    }

                    if (dropdownOptionsTextFuncs.Count == dropdown.options.Count)
                    {
                        LangAdjustables.RegisterDropdown(dropdown, dropdownOptionsTextFuncs);
                    }
                }
            });

            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>()
            {
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_videoSettingsTab/_backdrop_videoSettings/Scroll View/Viewport/Content/_cell_fieldOfView/Button/Text", "SETTINGS_BUTTON_RESET" },
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_videoSettingsTab/_backdrop_videoSettings/Scroll View/Viewport/Content/_cell_cameraSmoothing/Button_01/Text", "SETTINGS_BUTTON_RESET" },
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_videoSettingsTab/_backdrop_videoSettings/Scroll View/Viewport/Content/_cell_cameraHoriz/Button_01/Text", "SETTINGS_BUTTON_RESET" },
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_videoSettingsTab/_backdrop_videoSettings/Scroll View/Viewport/Content/_cell_cameraVert/Button_01/Text", "SETTINGS_BUTTON_RESET" },
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_inputSettingsTab/_backdrop/Scroll View/Viewport/Content/_cell_cameraSensitivity/Button_01/Text", "SETTINGS_BUTTON_RESET" },
            });
        }

    }
}
