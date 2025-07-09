using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal class RTSettings
    {
        // settings
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Start))]
        [HarmonyPostfix]
        public static void SettingsManager_Start(SettingsManager __instance)
        {
            RTUtil.RemapAllTextUnderObject(__instance.gameObject, new Dictionary<string, string>()
            {
                { "Button_videoTab", "SETTINGS_TAB_BUTTON_VIDEO" },

                { "_header_GameEffectSettings", "SETTINGS_VIDEO_HEADER_GAME_EFFECT_SETTINGS" },
                { "_cell_proportionsToggle", "SETTINGS_VIDEO_CELL_PROPORTIONS_TOGGLE" },
                { "_cell_jiggleBonesToggle", "SETTINGS_VIDEO_CELL_JIGGLE_BONES_TOGGLE" },
                { "_cell_clearUnderclothesToggle", "SETTINGS_VIDEO_CELL_CLEAR_UNDERCLOTHES_TOGGLE" },

                { "_header_videoSettings", "SETTINGS_VIDEO_HEADER_VIDEO_SETTINGS" },
                { "_cell_fullscreenToggle", "SETTINGS_VIDEO_CELL_FULLSCREEN_TOGGLE" },
                { "_cell_verticalSync", "SETTINGS_VIDEO_CELL_VERTICAL_SYNC" },
                { "_cell_anisotropicFiltering", "SETTINGS_VIDEO_CELL_ANISOTROPIC_FILTERING" },
                { "_cell_screenResolution", "SETTINGS_VIDEO_CELL_SCREEN_RESOLUTION" },
                { "_cell_antiAliasing", "SETTINGS_VIDEO_CELL_ANTI_ALIASING" },
                { "_cell_textureFiltering", "SETTINGS_VIDEO_CELL_TEXTURE_FILTERING" },
                { "_cell_textureQuality", "SETTINGS_VIDEO_CELL_TEXTURE_QUALITY" },

                { "_header_CameraSettings", "SETTINGS_VIDEO_HEADER_CAMERA_SETTINGS" },
                { "_cell_fieldOfView", "SETTINGS_VIDEO_CELL_FIELD_OF_VIEW" },
                { "_cell_cameraSmoothing", "SETTINGS_VIDEO_CELL_CAMERA_SMOOTHING" },
                { "_cell_cameraHoriz", "SETTINGS_VIDEO_CELL_CAMERA_HORIZ" },
                { "_cell_cameraVert", "SETTINGS_VIDEO_CELL_CAMERA_VERT" },
                { "_cell_cameraRenderDistance", "SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE" },

                { "_header_PostProcessing", "SETTINGS_VIDEO_HEADER_POST_PROCESSING" },
                { "_cell_cameraBitcrushShader", "SETTINGS_VIDEO_CELL_CAMERA_BITCRUSH_SHADER" },
                { "_cell_cameraWaterEffect", "SETTINGS_VIDEO_CELL_CAMERA_WATER_EFFECT" },
                { "_cell_cameraShake", "SETTINGS_VIDEO_CELL_CAMERA_SHAKE" },
                { "_cell_weaponGlow", "SETTINGS_VIDEO_CELL_WEAPON_GLOW" },


                { "Button_audioTab", "SETTINGS_TAB_BUTTON_AUDIO" },

                { "_header_audioSettings", "SETTINGS_AUDIO_HEADER_AUDIO_SETTINGS" },
                { "_cell_masterVolume", "SETTINGS_AUDIO_CELL_MASTER_VOLUME" },
                { "_cell_muteApplication", "SETTINGS_AUDIO_CELL_MUTE_APPLICATION" },
                { "_cell_muteMusic", "SETTINGS_AUDIO_CELL_MUTE_MUSIC" },

                { "_header_audioChannelSettings", "SETTINGS_AUDIO_HEADER_AUDIO_CHANNEL_SETTINGS" },
                { "_cell_gameVolume", "SETTINGS_AUDIO_CELL_GAME_VOLUME" },
                { "_cell_guiVolume", "SETTINGS_AUDIO_CELL_GUI_VOLUME" },
                { "_cell_ambienceVolume", "SETTINGS_AUDIO_CELL_AMBIENCE_VOLUME" },
                { "_cell_musicVolume", "SETTINGS_AUDIO_CELL_MUSIC_VOLUME" },
                { "_cell_voiceVolume", "SETTINGS_AUDIO_CELL_VOICE_VOLUME" },


                { "Button_inputTab", "SETTINGS_TAB_BUTTON_INPUT" },

                { "Image_05", "SETTINGS_INPUT_HEADER_INPUT_SETTINGS" },
                { "_cell_axisType", "SETTINGS_INPUT_CELL_AXIS_TYPE" },
                { "_cell_resetToDefaults", "SETTINGS_BUTTON_RESET_TO_DEFAULTS" },

                { "Image_06", "SETTINGS_INPUT_HEADER_CAMERA_CONTROL" },
                { "_cell_cameraSensitivity", "SETTINGS_INPUT_CELL_CAMERA_SENSITIVITY" },
                { "_cell_invertXCameraAxis", "SETTINGS_INPUT_CELL_INVERT_X_CAMERA_AXIS" },
                { "_cell_invertYCameraAxis", "SETTINGS_INPUT_CELL_INVERT_Y_CAMERA_AXIS" },
                { "_cell_keybinding_37", "SETTINGS_INPUT_CELL_KEYBINDING_RESET_CAMERA" },

                { "Header_Movement", "SETTINGS_INPUT_HEADER_MOVEMENT" },
                { "_cell_keybinding_up", "SETTINGS_INPUT_CELL_KEYBINDING_UP" },
                { "_cell_keybinding_down", "SETTINGS_INPUT_CELL_KEYBINDING_DOWN" },
                { "_cell_keybinding_left", "SETTINGS_INPUT_CELL_KEYBINDING_LEFT" },
                { "_cell_keybinding_right", "SETTINGS_INPUT_CELL_KEYBINDING_RIGHT" },
                { "_cell_keybinding_jump", "SETTINGS_INPUT_CELL_KEYBINDING_JUMP" },
                { "_cell_keybinding_dash", "SETTINGS_INPUT_CELL_KEYBINDING_DASH" },

                { "Header_Strafing", "SETTINGS_INPUT_HEADER_STRAFING" },
                { "_cell_keybinding_lockDirection", "SETTINGS_INPUT_CELL_KEYBINDING_LOCK_DIRECTION" },
                { "_cell_strafeMode", "SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_MODE" },
                { "_cell_strafeWeapon", "SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_WEAPON" },
                { "_cell_strafeCasting", "SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_CASTING" },

                { "Header_Action", "SETTINGS_INPUT_HEADER_ACTION" },
                { "_cell_keybinding_attack", "SETTINGS_INPUT_CELL_KEYBINDING_ATTACK" },
                { "_cell_keybinding_chargeAttack", "SETTINGS_INPUT_CELL_KEYBINDING_CHARGE_ATTACK" },
                { "_cell_keybinding_block", "SETTINGS_INPUT_CELL_KEYBINDING_BLOCK" },
                { "_cell_keybinding_target", "SETTINGS_INPUT_CELL_KEYBINDING_TARGET" },
                { "_cell_keybinding_interact", "SETTINGS_INPUT_CELL_KEYBINDING_INTERACT" },
                { "_cell_keybinding_pvpFlag", "SETTINGS_INPUT_CELL_KEYBINDING_PVP_FLAG" },
                { "_cell_keybinding_skillSlot01", "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_01" },
                { "_cell_keybinding_skillSlot02", "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_02" },
                { "_cell_keybinding_skillSlot03", "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_03" },
                { "_cell_keybinding_skillSlot04", "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_04" },
                { "_cell_keybinding_skillSlot05", "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_05" },
                { "_cell_keybinding_skillSlot06", "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_06" },
                { "_cell_keybinding_recall", "SETTINGS_INPUT_CELL_KEYBINDING_RECALL" },
                { "_cell_keybinding_quickswapWeapon", "SETTINGS_INPUT_CELL_KEYBINDING_QUICKSWAP_WEAPON" },
                { "_cell_keybinding_sheatheWeapon", "SETTINGS_INPUT_CELL_KEYBINDING_SHEATHE_WEAPON" },
                { "_cell_keybinding_sit", "SETTINGS_INPUT_CELL_KEYBINDING_SIT" },

                { "Header_ConsumableSlots", "SETTINGS_INPUT_HEADER_CONSUMABLE_SLOTS" },
                { "_cell_keybinding_quickSlot01", "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_01" },
                { "_cell_keybinding_quickSlot02", "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_02" },
                { "_cell_keybinding_quickSlot03", "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_03" },
                { "_cell_keybinding_quickSlot04", "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_04" },
                { "_cell_keybinding_quickSlot05", "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_05" },

                { "Header_Interface", "SETTINGS_INPUT_HEADER_INTERFACE" },
                { "_cell_keybinding_38", "SETTINGS_INPUT_CELL_KEYBINDING_HOST_CONSOLE" },
                { "_cell_keybinding_lexicon", "SETTINGS_INPUT_CELL_KEYBINDING_LEXICON" },
                { "_cell_keybinding_tabMenu", "SETTINGS_INPUT_CELL_KEYBINDING_TAB_MENU" },
                { "_cell_keybinding_statsTab", "SETTINGS_INPUT_CELL_KEYBINDING_STATS_TAB" },
                { "_cell_keybinding_skillsTab", "SETTINGS_INPUT_CELL_KEYBINDING_SKILLS_TAB" },
                { "_cell_keybinding_itemTab", "SETTINGS_INPUT_CELL_KEYBINDING_ITEM_TAB" },
                { "_cell_keybinding_questTab", "SETTINGS_INPUT_CELL_KEYBINDING_QUEST_TAB" },
                { "_cell_keybinding_whoTab", "SETTINGS_INPUT_CELL_KEYBINDING_WHO_TAB" },
                { "_cell_keybinding_hideUI", "SETTINGS_INPUT_CELL_KEYBINDING_HIDE_UI" },


                { "Button_networkTab", "SETTINGS_TAB_BUTTON_NETWORK" },

                { "_header_uiSettings", "SETTINGS_NETWORK_HEADER_UI_SETTINGS" },
                { "_cell_displayCreepNametags", "SETTINGS_NETWORK_CELL_DISPLAY_CREEP_NAMETAGS" },
                { "_cell_displayGlobalNicknameTags", "SETTINGS_NETWORK_CELL_DISPLAY_GLOBAL_NICKNAME_TAGS" },
                { "_cell_displayLocalNametag", "SETTINGS_NETWORK_CELL_DISPLAY_LOCAL_NAMETAG" },
                { "_cell_displayHostTag", "SETTINGS_NETWORK_CELL_DISPLAY_HOST_TAG" },
                { "_cell_hideDungeonMinimap", "SETTINGS_NETWORK_CELL_HIDE_DUNGEON_MINIMAP" },
                { "_cell_hideFPSCounter", "SETTINGS_NETWORK_CELL_HIDE_FPS_COUNTER" },
                { "_cell_hidePingCounter", "SETTINGS_NETWORK_CELL_HIDE_PING_COUNTER" },
                { "_cell_hideStatPointCounter", "SETTINGS_NETWORK_CELL_HIDE_STAT_POINT_COUNTER" },
                { "_cell_hideSkillPointCounter", "SETTINGS_NETWORK_CELL_HIDE_SKILL_POINT_COUNTER" },

                { "_header_clientSettings", "SETTINGS_NETWORK_HEADER_CLIENT_SETTINGS" },
                { "_cell_enablePvPOnMapEnter", "SETTINGS_NETWORK_CELL_ENABLE_PVP_ON_MAP_ENTER" },


                { "Button_cancelSettings", "SETTINGS_BUTTON_CANCEL" },
                { "Button_applySettings", "SETTINGS_BUTTON_APPLY" },
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
                        if (Localyssation.defaultLanguage.strings.TryGetValue(dropdownOptionKey, out var dropdownOptionText))
                            dropdownOptionsTextFuncs.Add(LangAdjustables.GetStringFunc(dropdownOptionKey, option.text));
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
