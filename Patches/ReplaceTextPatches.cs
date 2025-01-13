using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Localyssation.Patches
{
    internal static class ReplaceTextPatches
    {
        // general utils

        /// <summary>
        /// Creates a CodeMatcher that remaps strings found in the game with localised versions found in the current language.
        /// </summary>
        /// <param name="instructions">IEnumerable provided by the Harmony transpiler.</param>
        /// <param name="stringReplacements">Key-value pairs of in-game strings to replace and language keys to replace with.</param>
        /// <returns>The generated CodeMatcher.</returns>
        internal static IEnumerable<CodeInstruction> SimpleStringReplaceTranspiler(IEnumerable<CodeInstruction> instructions, Dictionary<string, string> stringReplacements)
        {
            var replacedStrings = new List<string>();
            return new CodeMatcher(instructions).MatchForward(false,
                new CodeMatch((instr) => instr.opcode == OpCodes.Ldstr && stringReplacements.ContainsKey((string)instr.operand) && !replacedStrings.Contains((string)instr.operand)))
                .Repeat(matcher =>
                {
                    var key = (string)matcher.Instruction.operand;
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(Transpilers.EmitDelegate<Func<string, string>>((origString) =>
                    {
                        return Localyssation.GetString(stringReplacements[key], key);
                    }));
                    replacedStrings.Add(key);
                }).InstructionEnumeration();
        }

        /// <summary>
        /// Gets the operand from the current CodeMatcher instruction. If none found, tries to parse the last symbol of the instruction's OpCode name as an integer.
        /// </summary>
        /// <param name="matcher">The CodeMatcher to get the instruction operand from.</param>
        /// <returns>The int operand.</returns>
        internal static int GetIntOperand(CodeMatcher matcher)
        {
            var result = matcher.Operand;
            if (result == null)
                result = int.Parse(matcher.Opcode.Name.Substring(matcher.Opcode.Name.Length - 1));
            return (int)result;
        }

        /// <summary>
        /// Remaps all UnityEngine.UI.Text instances found in this object, and under all of its children, with localised variants if the GameObject names match remap keys.
        /// </summary>
        /// <param name="gameObject">The GameObject to find Text instances in.</param>
        /// <param name="textRemaps">Key-value pairs of GameObject names to find and language keys to replace their text with.</param>
        /// <param name="onRemap">A method called on a successful remap.</param>
        public static void RemapAllTextUnderObject(GameObject gameObject, Dictionary<string, string> textRemaps, Action<Transform, string> onRemap = null)
        {
            bool TryRemapSingle(Transform lookupNameTransform, Text text)
            {
                if (textRemaps.TryGetValue(lookupNameTransform.name, out var key))
                {
                    LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc(key, text.text));
                    if (onRemap != null) onRemap(lookupNameTransform, key);
                    return true;
                }
                return false;
            }

            foreach (var text in gameObject.GetComponentsInChildren<Text>())
            {
                if (!TryRemapSingle(text.transform, text))
                {
                    var textParent = text.transform.parent;
                    if (textParent) TryRemapSingle(textParent, text);
                }
            }
        }

        /// <summary>
        /// Remaps all UnityEngine.UI.Text instances found under the parent transform with localised variants if the child GameObject paths match remap keys.
        /// </summary>
        /// <param name="parentTransform">The Transform to find Text instances under.</param>
        /// <param name="textRemaps">Key-value pairs of GameObject paths to find and language keys to replace their text with.</param>
        /// <param name="onRemap">A method called on a successful remap.</param>
        public static void RemapChildTextsByPath(Transform parentTransform, Dictionary<string, string> textRemaps, Action<Transform, string> onRemap = null)
        {
            foreach (var textRemap in textRemaps)
            {
                var foundTransform = parentTransform.Find(textRemap.Key);
                if (foundTransform)
                {
                    var text = foundTransform.GetComponent<Text>();
                    if (text)
                    {
                        LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc(textRemap.Value, text.text));
                        if (onRemap != null) onRemap(foundTransform, textRemap.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Remaps all UnityEngine.UI.InputField instances' placeholder texts found in this object, and under all of its children, with localised variants if the in-game strings match remap keys.
        /// </summary>
        /// <param name="gameObject">The GameObject to find InputField instances in.</param>
        /// <param name="textRemaps">Key-value pairs of in-game strings to replace and language keys to replace with.</param>
        /// <param name="onRemap">A method called on a successful remap.</param>
        public static void RemapAllInputPlaceholderTextUnderObject(GameObject gameObject, Dictionary<string, string> textRemaps, Action<Transform, string> onRemap = null)
        {
            foreach (var inputField in gameObject.GetComponentsInChildren<InputField>())
            {
                if (inputField.placeholder)
                {
                    var text = inputField.placeholder.GetComponent<Text>();
                    if (text && textRemaps.TryGetValue(inputField.name, out var key))
                    {
                        LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc(key, text.text));
                        if (onRemap != null) onRemap(inputField.transform, key);
                    }
                }
            }
        }

        private static List<string> fallbackTextEditTags = new List<string>()
        {
            "scalefallback"
        };

        [HarmonyPatch(typeof(Text), nameof(Text.text), MethodType.Setter)]
        [HarmonyPrefix]
        public static void Text_set_text(Text __instance, ref string value)
        {
            // if fontSize is not provided for a <scale> tag,
            // it gets auto-replaced with a <scalefallback> tag that gets parsed here instead
            if (__instance != null && value != null && value.Contains("scalefallback"))
            {
                value = Localyssation.ApplyTextEditTags(value, __instance.fontSize, fallbackTextEditTags);
            }
        }

        [HarmonyPatch(typeof(Text), nameof(Text.OnEnable))]
        [HarmonyPostfix]
        public static void Text_OnEnable(Text __instance)
        {
            if (Localyssation.currentLanguage != null && __instance != null && __instance.font != null)
            {
                LangAdjustables.RegisterText(__instance);
            }
        }

        // main menu & character select
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Awake))]
        [HarmonyPostfix]
        public static void MainMenuManager_Awake(MainMenuManager __instance)
        {
            var parent = __instance.transform.parent;
            if (parent)
            {
                var obj_Canvas_MainMenu = parent.Find("_mainMenu/Canvas_MainMenu");
                if (obj_Canvas_MainMenu)
                {
                    var obj_dolly_selectBar = obj_Canvas_MainMenu.Find("_dolly_selectBar");
                    var obj_text_toolTipHelp = obj_Canvas_MainMenu.Find("_backdrop_lowBar/_text_toolTipHelp");
                    if (obj_dolly_selectBar)
                    {
                        RemapAllTextUnderObject(obj_dolly_selectBar.gameObject, new Dictionary<string, string>()
                        {
                            { "_button_singleplay", "MAIN_MENU_BUTTON_SINGLEPLAY" },
                            { "_button_multiplay", "MAIN_MENU_BUTTON_MULTIPLAY" },
                            { "_button_settings", "MAIN_MENU_BUTTON_SETTINGS" },
                            { "_button_quit", "MAIN_MENU_BUTTON_QUIT" }
                        }, (textParent, key) =>
                        {
                            if (!obj_text_toolTipHelp) return;
                            var tooltipText = obj_text_toolTipHelp.GetComponent<Text>();

                            var eventTrigger = textParent.GetComponent<EventTrigger>();
                            if (eventTrigger)
                            {
                                for (var i = 0; i < eventTrigger.triggers.Count; i++)
                                {
                                    var trigger = eventTrigger.triggers[i];
                                    foreach (var call in trigger.callback.m_PersistentCalls.m_Calls)
                                    {
                                        if (call.methodName == "set_text" && call.arguments.stringArgument != "" && call.target == tooltipText)
                                        {
                                            var newTrigger = new EventTrigger.Entry();
                                            newTrigger.eventID = trigger.eventID;
                                            newTrigger.callback.AddListener((_) =>
                                            {
                                                tooltipText.text = Localyssation.GetString($"{key}_TOOLTIP", call.arguments.stringArgument, tooltipText.fontSize);
                                            });
                                            eventTrigger.triggers.Add(newTrigger);
                                            return;
                                        }
                                    }
                                }
                            }
                        });
                    }
                }

                var obj_Canvas_characterSelect = parent.Find("_characterSelectMenu/Canvas_characterSelect");
                if (obj_Canvas_characterSelect)
                {
                    RemapAllTextUnderObject(obj_Canvas_characterSelect.gameObject, new Dictionary<string, string>()
                    {
                        { "_text_header", "CHARACTER_SELECT_HEADER" },
                        { "_button_createCharacter", "CHARACTER_SELECT_BUTTON_CREATE_CHARACTER" },
                        { "_button_deleteCharacter", "CHARACTER_SELECT_BUTTON_DELETE_CHARACTER" },
                        { "_button_select", "CHARACTER_SELECT_BUTTON_SELECT_CHARACTER" },
                        { "_button_return", "CHARACTER_SELECT_BUTTON_RETURN" },
                        { "_text_characterDeletePrompt", "CHARACTER_SELECT_CHARACTER_DELETE_PROMPT_TEXT" },
                        { "_button_confirmDeleteCharacter", "CHARACTER_SELECT_CHARACTER_DELETE_BUTTON_CONFIRM" },
                        { "_button_deletePrompt_return", "CHARACTER_SELECT_CHARACTER_DELETE_BUTTON_RETURN" },
                    });

                    RemapAllInputPlaceholderTextUnderObject(obj_Canvas_characterSelect.gameObject, new Dictionary<string, string>()
                    {
                        { "_input_characterDeleteConfirm", "CHARACTER_SELECT_CHARACTER_DELETE_PROMPT_PLACEHOLDER_TEXT" }
                    });
                }

                var obj_Canvas_characterCreation = parent.Find("_characterSelectMenu/Canvas_characterCreation");
                if (obj_Canvas_characterCreation)
                {
                    RemapAllTextUnderObject(obj_Canvas_characterCreation.gameObject, new Dictionary<string, string>()
                    {
                        { "_text_header", "CHARACTER_CREATION_HEADER" },
                        { "_header_raceName_01", "CHARACTER_CREATION_HEADER" },
                        { "_header_initialSkill", "CHARACTER_CREATION_RACE_DESCRIPTOR_HEADER_INITIAL_SKILL" },
                        { "_button_createCharacter", "CHARACTER_CREATION_BUTTON_CREATE_CHARACTER" },
                        { "_button_return", "CHARACTER_CREATION_BUTTON_RETURN" },
                    });

                    RemapAllInputPlaceholderTextUnderObject(obj_Canvas_characterSelect.gameObject, new Dictionary<string, string>()
                    {
                        { "_input_characterName", "CHARACTER_CREATION_CHARACTER_NAME_PLACEHOLDER_TEXT" }
                    });

                    var customizer_color = obj_Canvas_characterCreation.transform.Find("_dolly_customizer/_customizer_color");
                    var customizer_head = obj_Canvas_characterCreation.transform.Find("_dolly_customizer/_customizer_head");
                    var customizer_body = obj_Canvas_characterCreation.transform.Find("_dolly_customizer/_customizer_body");
                    var customizer_trait = obj_Canvas_characterCreation.transform.Find("_dolly_customizer/_customizer_trait");
                    if (customizer_color)
                        RemapAllTextUnderObject(customizer_color.gameObject, new Dictionary<string, string>()
                        {
                            { "_customizer_header", "CHARACTER_CREATION_CUSTOMIZER_HEADER_COLOR" },
                            { "Image_01", "CHARACTER_CREATION_CUSTOMIZER_COLOR_BODY_HEADER" },
                            { "_characterButtonSelector", "CHARACTER_CREATION_CUSTOMIZER_COLOR_BODY_TEXTURE" },
                            { "Image", "CHARACTER_CREATION_CUSTOMIZER_COLOR_HAIR_HEADER" },
                            { "Toggle_lockColor", "CHARACTER_CREATION_CUSTOMIZER_COLOR_HAIR_LOCK_COLOR" },
                            { "_button_defaultColor", "CHARACTER_CREATION_BUTTON_SET_TO_DEFAULTS" },
                        });
                    if (customizer_head)
                        RemapAllTextUnderObject(customizer_head.gameObject, new Dictionary<string, string>()
                        {
                            { "_customizer_header", "CHARACTER_CREATION_CUSTOMIZER_HEADER_HEAD" },
                            { "_characterSlider_headWidth", "CHARACTER_CREATION_CUSTOMIZER_HEAD_HEAD_WIDTH" },
                            { "_characterSlider_headMod", "CHARACTER_CREATION_CUSTOMIZER_HEAD_HEAD_MOD" },
                            { "_characterSlider_voicePitch", "CHARACTER_CREATION_CUSTOMIZER_HEAD_VOICE_PITCH" },
                            { "_characterButtons_hairStyle", "CHARACTER_CREATION_CUSTOMIZER_HEAD_HAIR_STYLE" },
                            { "_characterButtons_ears", "CHARACTER_CREATION_CUSTOMIZER_HEAD_EARS" },
                            { "_characterButtons_eyes", "CHARACTER_CREATION_CUSTOMIZER_HEAD_EYES" },
                            { "_characterButtons_mouth", "CHARACTER_CREATION_CUSTOMIZER_HEAD_MOUTH" },
                        });
                    if (customizer_body)
                        RemapAllTextUnderObject(customizer_body.gameObject, new Dictionary<string, string>()
                        {
                            { "_customizer_header", "CHARACTER_CREATION_CUSTOMIZER_HEADER_BODY" },
                            { "_characterSlider_height", "CHARACTER_CREATION_CUSTOMIZER_BODY_HEIGHT" },
                            { "_characterSlider_width", "CHARACTER_CREATION_CUSTOMIZER_BODY_WIDTH" },
                            { "_characterSlider_chest", "CHARACTER_CREATION_CUSTOMIZER_BODY_CHEST" },
                            { "_characterSlider_arms", "CHARACTER_CREATION_CUSTOMIZER_BODY_ARMS" },
                            { "_characterSlider_belly", "CHARACTER_CREATION_CUSTOMIZER_BODY_BELLY" },
                            { "_characterSlider_bottom", "CHARACTER_CREATION_CUSTOMIZER_BODY_BOTTOM" },
                            { "_characterButtonSelector_tail", "CHARACTER_CREATION_CUSTOMIZER_BODY_TAIL" },
                            { "_toggle_leftHanded", "CHARACTER_CREATION_CUSTOMIZER_BODY_TOGGLE_LEFT_HANDED" },
                            { "_button_defaultBody", "CHARACTER_CREATION_BUTTON_SET_TO_DEFAULTS" },
                        });
                    if (customizer_trait)
                        RemapAllTextUnderObject(customizer_trait.gameObject, new Dictionary<string, string>()
                        {
                            { "_customizer_header", "CHARACTER_CREATION_CUSTOMIZER_HEADER_TRAIT" },
                            { "_header_equipment", "CHARACTER_CREATION_CUSTOMIZER_TRAIT_EQUIPMENT" },
                            { "_selector_weaponLoadout", "CHARACTER_CREATION_CUSTOMIZER_TRAIT_WEAPON_LOADOUT" },
                            { "_selector_gearDye", "CHARACTER_CREATION_CUSTOMIZER_TRAIT_GEAR_DYE" },
                            { "_header_attributes", "CHARACTER_CREATION_CUSTOMIZER_TRAIT_ATTRIBUTES" },
                            { "_text_strengthAttribute", "STAT_ATTRIBUTE_STRENGTH_NAME" },
                            { "_text_mindAttribute", "STAT_ATTRIBUTE_MIND_NAME" },
                            { "_text_dexterityAttribute", "STAT_ATTRIBUTE_DEXTERITY_NAME" },
                            { "_text_vitalityAttribute", "STAT_ATTRIBUTE_VITALITY_NAME" },
                            { "_button_resetAtbPoints", "CHARACTER_CREATION_CUSTOMIZER_TRAIT_RESET_ATTRIBUTE_POINTS" },
                        });
                }
            }
        }

        [HarmonyPatch(typeof(CharacterSelectManager), nameof(CharacterSelectManager.Handle_HeaderText))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> CharacterSelectManager_Handle_HeaderText_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "Singleplayer", "CHARACTER_SELECT_HEADER_GAME_MODE_SINGLEPLAYER" },
                { "Host Game (Public)", "CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_PUBLIC" },
                { "Host Game (Friends)", "CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_FRIENDS" },
                { "Host Game (Private)", "CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_PRIVATE" },
                { "Join Game", "CHARACTER_SELECT_HEADER_GAME_MODE_JOIN_MULTIPLAYER" },
                { "Lobby Query", "CHARACTER_SELECT_HEADER_GAME_MODE_LOBBY_QUERY" },
            });
        }

        [HarmonyPatch(typeof(CharacterSelectListDataEntry), nameof(CharacterSelectListDataEntry.Update))]
        [HarmonyPostfix]
        public static void CharacterSelectListDataEntry_Update(CharacterSelectListDataEntry __instance)
        {
            if (__instance._characterFileData._isEmptySlot)
            {
                __instance._characterNicknameText.text = Localyssation.GetString("CHARACTER_SELECT_DATA_ENTRY_EMPTY_SLOT", __instance._characterNicknameText.text, __instance._characterNicknameText.fontSize);
            }
            else
            {
                var fontSize = __instance._characterInfoText.fontSize;

                var raceName = "";
                var className = Localyssation.GetString("PLAYER_CLASS_EMPTY_NAME", GameManager._current._statLogics._emptyClassName, fontSize);

                var race = GameManager._current.LocateRace(__instance._characterFileData._appearanceProfile._setRaceTag);
                if (race) raceName = Localyssation.GetString($"{KeyUtil.GetForAsset(race)}_NAME", race._raceName, fontSize);

                if (!string.IsNullOrEmpty(__instance._characterFileData._statsProfile._classID))
                {
                    var playerClass = GameManager._current.LocateClass(__instance._characterFileData._statsProfile._classID);
                    if (playerClass) className = Localyssation.GetString($"{KeyUtil.GetForAsset(playerClass)}_NAME", playerClass._className, fontSize);
                }

                __instance._characterInfoText.text = string.Format(Localyssation.GetString(
                    "FORMAT_CHARACTER_SELECT_DATA_ENTRY_INFO",
                    __instance._characterInfoText.text,
                    __instance._characterInfoText.fontSize),
                    __instance._characterFileData._statsProfile._currentLevel,
                    raceName,
                    className);
            }
        }

        [HarmonyPatch(typeof(CharacterCreationManager), nameof(CharacterCreationManager.Handle_InterfaceParameters))]
        [HarmonyPostfix]
        public static void CharacterCreationManager_Handle_InterfaceParameters(CharacterCreationManager __instance)
        {
            var race = __instance._scriptablePlayerRaces[__instance._currentRaceSelected];
            if (race)
            {
                var key = KeyUtil.GetForAsset(race);

                __instance._raceDescriptionHeader.text = (Localyssation.GetString($"{key}_NAME", __instance._raceDescriptionHeader.text, __instance._raceDescriptionHeader.fontSize) ?? "");
                __instance._raceDescriptorField.text = (Localyssation.GetString($"{key}_DESCRIPTION", __instance._raceDescriptorField.text, __instance._raceDescriptorField.fontSize) ?? "");
                __instance._colorMiscTag.text = (Localyssation.GetString($"{key}_MISC", __instance._colorMiscTag.text, __instance._colorMiscTag.fontSize) ?? "");
                __instance._miscTag.text = (Localyssation.GetString($"{key}_MISC", __instance._miscTag.text, __instance._miscTag.fontSize) ?? "");
                if (race._racialSkills.Length >= 1)
                {
                    var skillKey = KeyUtil.GetForAsset(race._racialSkills[0]);
                    __instance._raceInitialSkillTag.text = (Localyssation.GetString($"{skillKey}_NAME", __instance._raceInitialSkillTag.text, __instance._raceInitialSkillTag.fontSize) ?? "");
                    __instance._raceInitialSkillDescriptor.text = (Localyssation.GetString($"{skillKey}_DESCRIPTION", __instance._raceInitialSkillDescriptor.text, __instance._raceInitialSkillDescriptor.fontSize) ?? "");
                }
            }
        }

        // settings
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Start))]
        [HarmonyPostfix]
        public static void SettingsManager_Start(SettingsManager __instance)
        {
            RemapAllTextUnderObject(__instance.gameObject, new Dictionary<string, string>()
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

            RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>()
            {
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_videoSettingsTab/_backdrop_videoSettings/Scroll View/Viewport/Content/_cell_fieldOfView/Button/Text", "SETTINGS_BUTTON_RESET" },
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_videoSettingsTab/_backdrop_videoSettings/Scroll View/Viewport/Content/_cell_cameraSmoothing/Button_01/Text", "SETTINGS_BUTTON_RESET" },
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_videoSettingsTab/_backdrop_videoSettings/Scroll View/Viewport/Content/_cell_cameraHoriz/Button_01/Text", "SETTINGS_BUTTON_RESET" },
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_videoSettingsTab/_backdrop_videoSettings/Scroll View/Viewport/Content/_cell_cameraVert/Button_01/Text", "SETTINGS_BUTTON_RESET" },
                { "Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_inputSettingsTab/_backdrop/Scroll View/Viewport/Content/_cell_cameraSensitivity/Button_01/Text", "SETTINGS_BUTTON_RESET" },
            });
        }

        // equipment
        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Apply_EquipStats))]
        [HarmonyPostfix]
        public static void EquipToolTip_Apply_EquipStats(EquipToolTip __instance, ScriptableEquipment _scriptEquip, ItemData _itemData)
        {
            if (_scriptEquip && !__instance._isGambleItem)
            {
                var key = KeyUtil.GetForAsset(_scriptEquip);

                var shownRarity = _scriptEquip._itemRarity;
                if (!string.IsNullOrWhiteSpace(_itemData._modifierTag) && GameManager._current.LocateStatModifier(_itemData._modifierTag))
                {
                    shownRarity += 1;
                }

                if (!string.IsNullOrEmpty(_scriptEquip._itemName))
                    __instance._toolTipName.text = __instance._toolTipName.text.Replace(_scriptEquip._itemName, Localyssation.GetString($"{key}_NAME", __instance._toolTipName.text, __instance._toolTipName.fontSize));
                __instance._toolTipSubName.text = string.Format(Localyssation.GetString(
                    "FORMAT_EQUIP_ITEM_RARITY",
                    __instance._toolTipSubName.text,
                    __instance._toolTipSubName.fontSize),
                    Localyssation.GetString(KeyUtil.GetForAsset(shownRarity), _scriptEquip._itemRarity.ToString(), __instance._toolTipSubName.fontSize));

                if (!string.IsNullOrEmpty(_scriptEquip._itemDescription))
                    __instance._toolTipDescription.text = Localyssation.GetString($"{key}_DESCRIPTION", __instance._toolTipDescription.text, __instance._toolTipDescription.fontSize);

                if (_scriptEquip._classRequirement)
                    __instance._equipClassRequirement.text = string.Format(Localyssation.GetString(
                        "FORMAT_EQUIP_CLASS_REQUIREMENT",
                        __instance._equipClassRequirement.text,
                        __instance._equipClassRequirement.fontSize),
                        Localyssation.GetString($"{KeyUtil.GetForAsset(_scriptEquip._classRequirement)}_NAME", __instance._equipClassRequirement.text, __instance._equipClassRequirement.fontSize));

                if (_scriptEquip.GetType() == typeof(ScriptableWeapon))
                {
                    var weapon = (ScriptableWeapon)_scriptEquip;

                    if (weapon._weaponConditionSlot._scriptableCondition)
                    {
                        __instance._toolTipDescription.text += string.Format(Localyssation.GetString(
                            "FORMAT_EQUIP_WEAPON_CONDITION",
                            __instance._toolTipDescription.text,
                            __instance._toolTipDescription.fontSize),
                            weapon._weaponConditionSlot._chance * 100f,
                            Localyssation.GetString($"{KeyUtil.GetForAsset(weapon._weaponConditionSlot._scriptableCondition)}_NAME", weapon._weaponConditionSlot._scriptableCondition._conditionName, __instance._toolTipDescription.fontSize));
                    }

                    if (Enum.TryParse<DamageType>(__instance._equipWeaponDamageType.text, out var damageType))
                        __instance._equipWeaponDamageType.text = __instance._equipWeaponDamageType.text.Replace(damageType.ToString(), Localyssation.GetString(KeyUtil.GetForAsset(damageType), damageType.ToString(), __instance._equipWeaponDamageType.fontSize));

                    if (weapon._combatElement)
                    {
                        if (!string.IsNullOrEmpty(weapon._combatElement._elementName))
                            __instance._equipElementText.text = __instance._equipElementText.text.Replace(weapon._combatElement._elementName, Localyssation.GetString($"{KeyUtil.GetForAsset(weapon._combatElement)}_NAME", weapon._combatElement._elementName, __instance._equipElementText.fontSize));
                    }
                    else
                    {
                        __instance._equipElementText.text = __instance._equipElementText.text.Replace("Normal", Localyssation.GetString("COMBAT_ELEMENT_NORMAL_NAME", "Normal", __instance._equipElementText.fontSize));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Apply_EquipStats))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EquipToolTip_Apply_EquipStats_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "Mystery Gear", "EQUIP_TOOLTIP_GAMBLE_ITEM_NAME" },
                { "[Unknown]", "EQUIP_TOOLTIP_GAMBLE_ITEM_RARITY" },
                { "???", "EQUIP_TOOLTIP_GAMBLE_ITEM_TYPE" },
                { "You can't really see what this is until you buy it.", "EQUIP_TOOLTIP_GAMBLE_ITEM_DESCRIPTION" },
                { "Lv-{0}", "FORMAT_EQUIP_LEVEL_REQUIREMENT" },
                { "Helm (Armor)", "EQUIP_TOOLTIP_TYPE_HELM" },
                { "Chestpiece (Armor)", "EQUIP_TOOLTIP_TYPE_CHESTPIECE" },
                { "Leggings (Armor)", "EQUIP_TOOLTIP_TYPE_LEGGINGS" },
                { "Cape (Armor)", "EQUIP_TOOLTIP_TYPE_CAPE" },
                { "Ring (Armor)", "EQUIP_TOOLTIP_TYPE_RING" },
                { "Shield", "EQUIP_TOOLTIP_TYPE_SHIELD" },
                { "<color=#efcc00>({0} - {1})</color> Damage", "FORMAT_EQUIP_STATS_DAMAGE_SCALED_POWERFUL" },
                { "<color=#c5e384>({0} - {1})</color> Damage", "FORMAT_EQUIP_STATS_DAMAGE_SCALED" },
                { "\n<color=grey>(Base Damage: {0} - {1})</color>", "FORMAT_EQUIP_STATS_DAMAGE_COMPARE_BASE" },
                { "({0} - {1}) Damage", "FORMAT_EQUIP_STATS_DAMAGE_UNSCALED" },
                { "Block threshold: {0} damage", "FORMAT_EQUIP_STATS_BLOCK_THRESHOLD" }
            });
        }

        // stats menu
        [HarmonyPatch(typeof(StatsMenuCell), nameof(StatsMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void StatsMenuCell_Cell_OnAwake(StatsMenuCell __instance)
        {
            RemapAllTextUnderObject(__instance.gameObject, new Dictionary<string, string>()
            {
                { "_text_statsHeader", "TAB_MENU_CELL_STATS_HEADER" },
                { "_text_attributePointCounter (1)", "TAB_MENU_CELL_STATS_ATTRIBUTE_POINT_COUNTER" },
                { "_button_applyAttributePoints", "TAB_MENU_CELL_STATS_BUTTON_APPLY_ATTRIBUTE_POINTS" },
            });
            RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>()
            {
                { "_infoStatPanel/_statInfoCell_nickName/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_NICK_NAME" },
                { "_infoStatPanel/_statInfoCell_raceName/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_RACE_NAME" },
                { "_infoStatPanel/_statInfoCell_className/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_CLASS_NAME" },
                { "_infoStatPanel/_statInfoCell_levelCounter/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_LEVEL_COUNTER" },
                { "_infoStatPanel/_statInfoCell_experience/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_EXPERIENCE" },

                { "_infoStatPanel/_statInfoCell_maxHealth/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAX_HEALTH" },
                { "_infoStatPanel/_statInfoCell_maxMana/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAX_MANA" },
                { "_infoStatPanel/_statInfoCell_maxStamina/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAX_STAMINA" },

                { "_infoStatPanel/_statInfoCell_attack/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_ATTACK" },
                { "_infoStatPanel/_statInfoCell_rangedPower/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_RANGED_POWER" },
                { "_infoStatPanel/_statInfoCell_physCritical/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_PHYS_CRITICAL" },

                { "_infoStatPanel/_statInfoCell_magicPow/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_POW" },
                { "_infoStatPanel/_statInfoCell_magicCrit/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_CRIT" },

                { "_infoStatPanel/_statInfoCell_defense/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_DEFENSE" },
                { "_infoStatPanel/_statInfoCell_magicDef/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_DEF" },

                { "_infoStatPanel/_statInfoCell_evasion/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_EVASION" },
                { "_infoStatPanel/_statInfoCell_moveSpd/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MOVE_SPD" },
            });
        }

        [HarmonyPatch(typeof(StatsMenuCell), nameof(StatsMenuCell.Apply_StatsCellData))]
        [HarmonyPostfix]
        public static void StatsMenuCell_Apply_StatsCellData(StatsMenuCell __instance)
        {
            if (!TabMenu._current._isOpen && !__instance._mainPlayer._bufferingStatus) return;

            if (!string.IsNullOrEmpty(__instance._mainPlayer._pVisual._playerAppearanceStruct._setRaceTag))
            {
                var race = GameManager._current.LocateRace(__instance._mainPlayer._pVisual._playerAppearanceStruct._setRaceTag);
                if (race)
                {
                    __instance._statsCell_raceTag.text = Localyssation.GetString($"{KeyUtil.GetForAsset(race)}_NAME", __instance._statsCell_raceTag.text, __instance._statsCell_raceTag.fontSize);
                }
            }

            if (__instance._mainPlayer._pStats._currentLevel >= GameManager._current._statLogics._maxMainLevel)
                __instance._statsCell_experience.text = Localyssation.GetString("EXP_COUNTER_MAX", __instance._statsCell_experience.text, __instance._statsCell_experience.fontSize);

            var classFontSize = __instance._statsCell_baseClassTag.fontSize;
            string classText;
            if (__instance._mainPlayer._pStats._class)
                classText = Localyssation.GetString($"{KeyUtil.GetForAsset(__instance._mainPlayer._pStats._class)}_NAME", __instance._mainPlayer._pStats._class._className, classFontSize);
            else
                classText = Localyssation.GetString("PLAYER_CLASS_EMPTY_NAME", GameManager._current._statLogics._emptyClassName, classFontSize);
            __instance._statsCell_baseClassTag.text = classText;
        }

        [HarmonyPatch(typeof(StatsMenuCell), nameof(StatsMenuCell.ToolTip_DisplayBaseStat))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> StatsMenuCell_ToolTip_DisplayBaseStat_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "<b>Base Stat:</b> <i>", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_BEGIN" },
                { "%</i> (Critical %)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_CRIT" },
                { "%</i> (Evasion %)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_EVASION" },
                { "{0}</i> (Attack Power)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_ATTACK_POW" },
                { "{0}</i> (Max Mana)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_MP" },
                { "{0}</i> (Max Health)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_HP" },
                { "{0}</i> (Dex Power)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_RANGE_POW" },
                { "%</i> (Magic Critical %)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_MAGIC_CRIT" },
                { "{0}</i> (Magic Defense)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_DEF" },
                { "{0}</i> (Defense)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_DEFENSE" },
                { "{0}</i> (Magic Power)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_POW" },
                { "{0}</i> (Max Stamina)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_STAM" },
            });
        }

        [HarmonyPatch(typeof(AttributeListDataEntry), nameof(AttributeListDataEntry.Handle_AttributeData))]
        [HarmonyPostfix]
        public static void AttributeListDataEntry_Handle_AttributeData(AttributeListDataEntry __instance)
        {
            if (!GameManager._current ||
                !Player._mainPlayer ||
                string.IsNullOrEmpty(__instance._pStats._playerAttributes[__instance._dataID]._attributeName))
                return;

            var key = KeyUtil.GetForAsset(__instance._gm._statLogics._statAttributes[__instance._dataID]);
            __instance._dataNameText.text = Localyssation.GetString($"{key}_NAME", __instance._dataNameText.text, __instance._dataNameText.fontSize);
        }

        [HarmonyPatch(typeof(AttributeListDataEntry), nameof(AttributeListDataEntry.Init_TooltipInfo))]
        [HarmonyPostfix]
        public static void AttributeListDataEntry_Init_TooltipInfo(AttributeListDataEntry __instance)
        {
            if (string.IsNullOrEmpty(__instance._scriptableAttribute._attributeDescriptor)) return;

            var key = KeyUtil.GetForAsset(__instance._scriptableAttribute);
            ToolTipManager._current.Apply_GenericToolTip(Localyssation.GetString($"{key}_DESCRIPTOR", __instance._scriptableAttribute._attributeDescriptor));
        }

        // skills menu
        [HarmonyPatch(typeof(SkillsMenuCell), nameof(SkillsMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void SkillsMenuCell_Cell_OnAwake(SkillsMenuCell __instance)
        {
            RemapAllTextUnderObject(__instance.gameObject, new Dictionary<string, string>()
            {
                { "_text_skillsHeader", "TAB_MENU_CELL_SKILLS_HEADER" },
            });
            RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>()
            {
                { "_backdrop_skillPoints/_text_skillPointsTag", "TAB_MENU_CELL_SKILLS_SKILL_POINT_COUNTER" },
                { "Content_noviceSkills/_skillsCell_skillListObject_recall/_text_skillRank", "SKILL_RANK_SOULBOUND" },
            }, (transform, key) =>
            {
                if (key == "TAB_MENU_CELL_SKILLS_SKILL_POINT_COUNTER")
                {
                    var text = transform.GetComponent<Text>();
                    if (text) text.alignment = TextAnchor.MiddleLeft;
                }
            });
        }

        [HarmonyPatch(typeof(SkillsMenuCell), nameof(SkillsMenuCell.Init_ClassTabTooltip))]
        [HarmonyPostfix]
        public static void SkillsMenuCell_Init_ClassTabTooltip(SkillsMenuCell __instance, int _tabValue)
        {
            switch (_tabValue)
            {
                case 0:
                    ToolTipManager._current.Apply_GenericToolTip(Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP_NOVICE"));
                    break;
                case 1:
                    var playerClass = Player._mainPlayer._pStats._class;
                    if (!playerClass) return;

                    var classKey = $"{KeyUtil.GetForAsset(playerClass)}_NAME";
                    if (Localyssation.currentLanguage.strings.ContainsKey(classKey + "_VARIANT_OF"))
                        classKey += "_VARIANT_OF";

                    ToolTipManager._current.Apply_GenericToolTip(string.Format(
                        Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP"),
                        Localyssation.GetString(classKey, playerClass._className)));
                    break;
            }
        }

        [HarmonyPatch(typeof(SkillsMenuCell), nameof(SkillsMenuCell.Handle_CellUpdate))]
        [HarmonyPostfix]
        public static void SkillsMenuCell_Handle_CellUpdate(SkillsMenuCell __instance)
        {
            if (!TabMenu._current._isOpen || !Player._mainPlayer) return;

            var txt = __instance._skillsCell_classHeader.text;
            var fontSize = __instance._skillsCell_classHeader.fontSize;
            switch (__instance._currentSkillTab)
            {
                case SkillTier.NOVICE:
                    txt = Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_HEADER_NOVICE", txt, fontSize);
                    break;
                case SkillTier.CLASS:
                    var classKey = $"{KeyUtil.GetForAsset(__instance._pStats._class)}_NAME";
                    if (Localyssation.currentLanguage.strings.ContainsKey(classKey + "_VARIANT_OF"))
                        classKey += "_VARIANT_OF";
                    txt = string.Format(
                        Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_HEADER", fontSize: fontSize),
                        Localyssation.GetString(classKey, __instance._pStats._class._className, fontSize));
                    break;
            }
            __instance._skillsCell_classHeader.text = txt;
        }

        [HarmonyPatch(typeof(SkillListDataEntry), nameof(SkillListDataEntry.Handle_SkillData))]
        [HarmonyPostfix]
        public static void SkillListDataEntry_Handle_SkillData(SkillListDataEntry __instance)
        {
            if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus || !__instance._scriptSkill) return;

            __instance._skillNameText.text = Localyssation.GetString(
                $"{KeyUtil.GetForAsset(__instance._scriptSkill)}_NAME",
                __instance._skillNameText.text,
                __instance._skillNameText.fontSize);
            if (__instance._skillRankText)
            {
                __instance._skillRankText.text = string.Format(
                    Localyssation.GetString("FORMAT_SKILL_RANK", __instance._skillRankText.text, __instance._skillRankText.fontSize),
                    __instance._skillStruct._rank,
                    __instance._scriptSkill._skillRanks.Length);
            }
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillStats))]
        [HarmonyPostfix]
        public static void SkillToolTip_Apply_SkillStats(SkillToolTip __instance)
        {
            if (!Player._mainPlayer || !__instance._scriptSkill) return;

            var key = KeyUtil.GetForAsset(__instance._scriptSkill);

            __instance._toolTipName.text = Localyssation.GetString($"{key}_NAME", __instance._toolTipName.text, __instance._toolTipName.fontSize);
            __instance._toolTipSubName.text = string.Format(
                Localyssation.GetString("FORMAT_SKILL_RANK", __instance._toolTipSubName.text, __instance._toolTipSubName.fontSize),
                __instance._skillStruct._rank,
                __instance._scriptSkill._skillRanks.Length);
            __instance._scaleTypeText.text = string.Format(
                Localyssation.GetString("FORMAT_SKILL_TOOLTIP_DAMAGE_TYPE", __instance._scaleTypeText.text, __instance._scaleTypeText.fontSize),
                Localyssation.GetString(KeyUtil.GetForAsset(__instance._scriptSkill._skillDamageType), __instance._scriptSkill._skillDamageType.ToString(), __instance._scaleTypeText.fontSize));
            if (!string.IsNullOrEmpty(__instance._scriptSkill._skillDescription))
                __instance._toolTipDescription.text = Localyssation.GetString($"{key}_DESCRIPTION", __instance._toolTipDescription.text, __instance._toolTipDescription.fontSize);

            var rankIndex = 0;
            if (__instance._skillStruct._rank > 0) rankIndex = __instance._skillStruct._rank - 1;

            if (__instance._scriptSkill._skillControlType != SkillControlType.Passive)
            {
                var requiredItem = __instance._scriptSkill._skillRanks[rankIndex]._requiredItem;
                if (requiredItem)
                {
                    __instance._itemCost.text = string.Format(
                        Localyssation.GetString("FORMAT_SKILL_TOOLTIP_ITEM_COST", __instance._itemCost.text, __instance._itemCost.fontSize),
                        __instance._scriptSkill._skillRanks[rankIndex]._requiredItemQuantity,
                        Localyssation.GetString($"{KeyUtil.GetForAsset(requiredItem)}_NAME", requiredItem._itemName, __instance._itemCost.fontSize));
                }
            }

            var passiveSkillText = __instance._passiveSkillTabObject.transform.Find("_passiveSkill_text").GetComponent<Text>();
            passiveSkillText.text = Localyssation.GetString("SKILL_TOOLTIP_PASSIVE", passiveSkillText.text, passiveSkillText.fontSize);
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillStats))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_SkillStats_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "{0} Mana", "FORMAT_SKILL_TOOLTIP_MANA_COST" },
                { "{0} Health", "FORMAT_SKILL_TOOLTIP_HEALTH_COST" },
                { "{0} Stamina", "FORMAT_SKILL_TOOLTIP_STAMINA_COST" },
                { "Instant Cast", "SKILL_TOOLTIP_CAST_TIME_INSTANT" },
                { "{0} sec Cast", "FORMAT_SKILL_TOOLTIP_CAST_TIME" },
                { "{0} sec Cooldown", "FORMAT_SKILL_TOOLTIP_COOLDOWN" },
            });
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillDecriptorInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_SkillDecriptorInfo_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "\n<color=white><i>[Next Rank]</i></color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_NEXT_RANK" },
                { "\n<color=white><i>[Rank {0}]</i></color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CURRENT_RANK" },
                { "<color=red>\n(Requires Lv. {0})</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_REQUIRED_LEVEL" },
                { "<color=yellow>{0} sec cooldown.</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_COOLDOWN" },
                { "<color=yellow>{0} sec cast time.</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME" },
                { "<color=yellow>instant cast time.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME_INSTANT" },
            });
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillDecriptorInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_SkillDecriptorInfo_Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SkillRanking), nameof(SkillRanking._rankDescriptor))));
            matcher.Advance(1);
            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SkillRanking), nameof(SkillRanking._rankDescriptor))));
            matcher.MatchForward(true,
                new CodeMatch(x => x.IsStloc()));
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg, 0),
                new CodeInstruction(OpCodes.Ldarg, 1),
                Transpilers.EmitDelegate<Func<string, SkillToolTip, int, string>>((oldString, __instance, _rank) =>
                {
                    var key = KeyUtil.GetForAsset(__instance._scriptSkill);
                    return Localyssation.GetString($"{key}_RANK_{_rank + 1}_DESCRIPTOR", oldString);
                }));
            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_ConditionRankInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_ConditionRankInfo_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { " <color=yellow>Cancels if hit.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_CANCEL_ON_HIT" },
                { " <color=yellow>Permanent.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_PERMANENT" },
                { " <color=yellow>Lasts for {0} seconds.</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_DURATION" },
                { " <color=yellow>Stackable.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_STACKABLE" },
                { " <color=yellow>Refreshes when re-applied.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_REFRESHABLE" },
            });
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_ConditionRankInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_ConditionRankInfo_Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableCondition), nameof(ScriptableCondition._conditionDescription))));
            matcher.Advance(1);
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg, 0),
                new CodeInstruction(OpCodes.Ldarg, 1),
                Transpilers.EmitDelegate<Func<string, SkillToolTip, int, string>>((oldString, __instance, _rank) =>
                {
                    var condition = __instance._scriptSkill._skillRanks[_rank]._selfConditionOutput;
                    var key = KeyUtil.GetForAsset(condition);
                    return Localyssation.GetString($"{key}_{condition._conditionRank}_DESCRIPTION", oldString);
                }));
            return matcher.InstructionEnumeration();
        }

        // quests
        [HarmonyPatch(typeof(QuestListDataEntry), nameof(QuestListDataEntry.Update))]
        [HarmonyPostfix]
        public static void QuestListDataEntry_Update(QuestListDataEntry __instance)
        {
            var key = KeyUtil.GetForAsset(__instance._scriptableQuest);

            var formattedQuestString = Localyssation.GetString($"{key}_NAME", __instance._scriptableQuest._questName, __instance._dataNameText.fontSize);

            if (__instance._scriptableQuest._questSubType == QuestSubType.NONE ||
                __instance._scriptableQuest._questSubType == QuestSubType.MAIN_QUEST)
            {
                formattedQuestString += " " + string.Format(
                    Localyssation.GetString("FORMAT_QUEST_REQUIRED_LEVEL", fontSize: __instance._dataNameText.fontSize),
                    __instance._scriptableQuest._questLevel);

                var styleTag = __instance._dataNameText.text.Substring(0, __instance._dataNameText.text.IndexOf(">") + 1);
                __instance._dataNameText.text = styleTag + formattedQuestString + "</color>";
            }
            else
            {
                formattedQuestString += " " + Localyssation.GetString(
                    $"QUEST_TYPE_{__instance._scriptableQuest._questSubType.ToString().ToUpper()}",
                    fontSize: __instance._dataNameText.fontSize);

                __instance._dataNameText.text = $"<color=#f7e98e>{formattedQuestString}</color>";
            }
        }

        [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Handle_CellUpdate))]
        [HarmonyPostfix]
        public static void QuestMenuCell_Handle_CellUpdate(QuestMenuCell __instance)
        {
            if (!Player._mainPlayer) return;

            __instance._questLogCounterText.text = string.Format(
                Localyssation.GetString("FORMAT_QUEST_MENU_CELL_QUEST_LOG_COUNTER", __instance._questLogCounterText.text, __instance._questLogCounterText.fontSize),
                __instance._pQuest._questProgressData.Count,
                __instance._pQuest._questLogLimit);

            var finishedQuestCount = 0;
            if (ProfileDataManager._current._characterFile._questProgressProfile._finishedQuests != null)
                finishedQuestCount = ProfileDataManager._current._characterFile._questProgressProfile._finishedQuests.Length;
            __instance._finishedQuestCounterText.text = string.Format(
                Localyssation.GetString("FORMAT_QUEST_MENU_CELL_FINISHED_QUEST_COUNTER", __instance._finishedQuestCounterText.text, __instance._finishedQuestCounterText.fontSize),
                finishedQuestCount);

            var errandsStr = "";
            if (__instance._pQuest._questProgressData.Count > 0 && __instance._selectedQuest)
            {
                var acceptedQuestIndex = 0;
                while (acceptedQuestIndex < __instance._pQuest._questProgressData.Count && !QuestTrackerManager._current._refreshingElements)
                {
                    var questProgress = __instance._pQuest._questProgressData[acceptedQuestIndex];
                    if (questProgress._questTag == __instance._selectedQuest._questName)
                    {
                        if (questProgress._questComplete)
                        {
                            var key = KeyUtil.GetForAsset(__instance._selectedQuest);
                            var local = Localyssation.GetString($"{key}_COMPLETE_RETURN_MESSAGE", __instance._selectedQuest._questCompleteReturnMessage, __instance._questErrandsText.fontSize);
                            errandsStr = errandsStr.Insert(0, $"<color=yellow>{local}</color>\n\n");
                        }
                        errandsStr += QuestTrackerManager._current._questTrackElements[acceptedQuestIndex]._trackElementText.text;
                    }
                    acceptedQuestIndex++;
                }
            }
            __instance._questErrandsText.text = errandsStr;
        }

        [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Select_QuestSlot))]
        [HarmonyPostfix]
        public static void QuestMenuCell_Select_QuestSlot(QuestMenuCell __instance, ScriptableQuest _scriptQuest)
        {
            var key = KeyUtil.GetForAsset(_scriptQuest);

            __instance._questHeaderText.text = Localyssation.GetString($"{key}_NAME", __instance._questHeaderText.text, __instance._questHeaderText.fontSize)
                + " " + string.Format(
                    Localyssation.GetString("FORMAT_QUEST_REQUIRED_LEVEL", fontSize: __instance._questHeaderText.fontSize),
                    _scriptQuest._questLevel);

            __instance._questSummaryText.text = Localyssation.GetString($"{key}_DESCRIPTION", __instance._questSummaryText.text, __instance._questSummaryText.fontSize);

            int expReward = (int)(((int)GameManager._current._statLogics._experienceCurve.Evaluate(_scriptQuest._questLevel)) * _scriptQuest._questExperiencePercentage);
            if (expReward > 0)
            {
                __instance._rewardsPanelText_experience.text = string.Format(
                    Localyssation.GetString("FORMAT_QUEST_MENU_CELL_REWARD_EXP", __instance._rewardsPanelText_experience.text, __instance._rewardsPanelText_experience.fontSize),
                    expReward);
            }
            if (_scriptQuest._questCurrencyReward > 0)
            {
                __instance._rewardsPanelText_currency.text = string.Format(
                    Localyssation.GetString("FORMAT_QUEST_MENU_CELL_REWARD_CURRENCY", __instance._rewardsPanelText_currency.text, __instance._rewardsPanelText_currency.fontSize),
                    expReward);
            }
        }

        [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Clear_DisplayQuestData))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestMenuCell_Clear_DisplayQuestData_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "No Quests in Quest Log.", "QUEST_MENU_SUMMARY_NO_QUESTS" },
                { "Select a Quest.", "QUEST_MENU_HEADER_UNSELECTED" },
            });
        }

        [HarmonyPatch(typeof(QuestMenuCellSlot), nameof(QuestMenuCellSlot.Update))]
        [HarmonyPostfix]
        public static void QuestMenuCellSlot_Update(QuestMenuCellSlot __instance)
        {
            if (__instance._scriptQuest)
            {
                var fontSize = __instance._slotTag.fontSize;
                var questName = Localyssation.GetString($"{KeyUtil.GetForAsset(__instance._scriptQuest)}_NAME", __instance._scriptQuest._questName, fontSize);
                var levelRequirementStr = string.Format(
                    Localyssation.GetString("FORMAT_QUEST_REQUIRED_LEVEL", fontSize: fontSize),
                    __instance._scriptQuest._questLevel);

                __instance._slotTag.text = $"{questName}\n{levelRequirementStr}";
                switch (__instance._scriptQuest._questSubType)
                {
                    case QuestSubType.MAIN_QUEST:
                        __instance._slotTag.text = $"<color=cyan>{questName}</color>\n<color=cyan>{levelRequirementStr}</color>";
                        break;
                    case QuestSubType.CLASS:
                        __instance._slotTag.text = $"<color=#f7e98e>{questName}</color>\n<color=#f7e98e>{Localyssation.GetString("QUEST_TYPE_CLASS", null, fontSize)}</color>";
                        break;
                    case QuestSubType.MASTERY:
                        __instance._slotTag.text = $"<color=#f7e98e>{questName}</color>\n<color=#f7e98e>{Localyssation.GetString("QUEST_TYPE_MASTERY", null, fontSize)}</color>";
                        break;
                }
            }
            else
            {
                __instance._slotTag.text = Localyssation.GetString("QUEST_MENU_CELL_SLOT_EMPTY", __instance._slotTag.text, __instance._slotTag.fontSize);
            }
        }

        [HarmonyPatch(typeof(QuestSelectionManager), nameof(QuestSelectionManager.Handle_Expbar))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestSelectionManager_Handle_Expbar_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "MAX", "EXP_COUNTER_MAX" },
            });
        }

        [HarmonyPatch(typeof(QuestSelectionManager), nameof(QuestSelectionManager.Handle_QuestSelectionConditions))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestSelectionManager_Handle_QuestSelectionConditions_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "Quest Incomplete", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_INCOMPLETE" },
                { "Complete Quest", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_TURN_IN" },
                { "Select a Quest", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_UNSELECTED" },
            });
        }

        [HarmonyPatch(typeof(QuestSelectionManager), nameof(QuestSelectionManager.Select_QuestEntry))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestSelectionManager_Select_QuestEntry_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "Accept Quest", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_ACCEPT" },
                { "Quest Locked", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_LOCKED" },
            });
        }

        internal static string GetCreepKillRequirementText(ScriptableCreep creep, int requirement, int fontSize = -1)
        {
            var formatKey = "FORMAT_QUEST_PROGRESS_CREEPS_KILLED";
            var creepKey = $"{KeyUtil.GetForAsset(creep)}_NAME";
            if (requirement > 1)
            {
                if (Localyssation.currentLanguage.strings.ContainsKey($"{creepKey}_VARIANT_{requirement}"))
                    creepKey = $"{creepKey}_VARIANT_{requirement}";
                else if (Localyssation.currentLanguage.strings.ContainsKey($"{creepKey}_VARIANT_MANY"))
                    creepKey = $"{creepKey}_VARIANT_MANY";

                if (Localyssation.currentLanguage.strings.ContainsKey($"{formatKey}_VARIANT_{requirement}"))
                    formatKey = $"{formatKey}_VARIANT_{requirement}";
                else if (Localyssation.currentLanguage.strings.ContainsKey($"{formatKey}_VARIANT_MANY"))
                    formatKey = $"{formatKey}_VARIANT_MANY";
            }
            if (Localyssation.currentLanguage.strings.ContainsKey($"{creepKey}_VARIANT_QUEST_KILLED"))
                creepKey = $"{creepKey}_VARIANT_QUEST_KILLED";

            return string.Format(
                Localyssation.GetString(formatKey, fontSize: fontSize),
                Localyssation.GetString(creepKey, fontSize: fontSize));
        }
        [HarmonyPatch(typeof(QuestTrackElement), nameof(QuestTrackElement.Handle_QuestTrackInfo))]
        [HarmonyPostfix]
        public static void QuestTrackElement_Handle_QuestTrackInfo(QuestTrackElement __instance)
        {
            var key = KeyUtil.GetForAsset(__instance._scriptQuest);
            if (!string.IsNullOrEmpty(__instance._scriptQuest._questName))
                __instance._trackQuestNameText.text = __instance._trackQuestNameText.text.Replace(__instance._scriptQuest._questName, Localyssation.GetString($"{key}_NAME", __instance._scriptQuest._questName, __instance._trackQuestNameText.fontSize));

            var playerQuesting = Player._mainPlayer.GetComponent<PlayerQuesting>();
            if (playerQuesting._questProgressData.Count > 0)
            {
                var questProgressData = playerQuesting._questProgressData[__instance._questIndex];

                var trackElementText = __instance._trackElementText.text.Split(new string[] { "\n" }, StringSplitOptions.None);
                var c = 0;
                var fontSize = __instance._trackElementText.fontSize;
                void ReplaceTrackElementText(string newText, int progressCurrent, int progressMax)
                {
                    var styleTag = trackElementText[c].Substring(0, trackElementText[c].IndexOf(">") + 1);
                    var formattedQuestString = string.Format(
                        Localyssation.GetString("FORMAT_QUEST_PROGRESS", fontSize: fontSize),
                        newText, progressCurrent, progressMax);
                    trackElementText[c] = styleTag + formattedQuestString + "</color>";
                    c++;
                }

                for (var i = 0; i < __instance._scriptQuest._questObjective._questItemRequirements.Length; i++)
                {
                    var questItemRequirement = __instance._scriptQuest._questObjective._questItemRequirements[i];
                    var itemKey = $"{KeyUtil.GetForAsset(questItemRequirement._questItem)}_NAME";
                    ReplaceTrackElementText(Localyssation.GetString(itemKey, questItemRequirement._questItem._itemName, fontSize), questProgressData._itemProgressValues[i], questItemRequirement._itemsNeeded);
                }
                for (var i = 0; i < __instance._scriptQuest._questObjective._questCreepRequirements.Length; i++)
                {
                    var questCreepRequirement = __instance._scriptQuest._questObjective._questCreepRequirements[i];
                    ReplaceTrackElementText(GetCreepKillRequirementText(questCreepRequirement._questCreep, questCreepRequirement._creepsKilled, fontSize), questProgressData._creepKillProgressValues[i], questCreepRequirement._creepsKilled);
                }
                for (var i = 0; i < __instance._scriptQuest._questObjective._questTriggerRequirements.Length; i++)
                {
                    var questTriggerRequirement = __instance._scriptQuest._questObjective._questTriggerRequirements[i];
                    var triggerKey = KeyUtil.GetForAsset(questTriggerRequirement);
                    ReplaceTrackElementText($"{Localyssation.GetString(triggerKey + "_PREFIX", questTriggerRequirement._prefix, fontSize)} {Localyssation.GetString(triggerKey + "_SUFFIX", questTriggerRequirement._suffix, fontSize)}", questProgressData._triggerProgressValues[i], questTriggerRequirement._triggerEmitsNeeded);
                }

                __instance._trackElementText.text = string.Join("\n", trackElementText);
            }
        }

        // dialog
        [HarmonyPatch(typeof(DialogManager), nameof(DialogManager.Start_Dialog))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DialogManager_Start_Dialog_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog), nameof(Dialog._dialogInput))))
                .MatchBack(true,
                    new CodeMatch(x => x.IsLdloc()));

            var dialog_pos = GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(DialogManager), nameof(DialogManager._dialogSentences))))
            .MatchForward(true,
                new CodeMatch(x => (x.opcode == OpCodes.Call || x.opcode == OpCodes.Callvirt) && ((MethodInfo)x.operand).Name == "Enqueue"));

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldloc, dialog_pos),
                Transpilers.EmitDelegate<Func<string, DialogManager, DialogBranch, Dialog, string>>((oldString, __instance, dialogBranch, dialog) =>
                {
                    if (__instance._scriptableDialog)
                    {
                        var key = KeyUtil.GetForAsset(__instance._scriptableDialog);

                        string GetInputKey(string keySuffixBranch)
                        {
                            var dialogIndex = Array.IndexOf(dialogBranch.dialogs, dialog);
                            var inputKey = $"{key}_{keySuffixBranch}_DIALOG_{dialogIndex}_INPUT";

                            if (dialog._altInputs != null && dialog._altInputs.Length != 0)
                                inputKey += $"_ALT_{UnityEngine.Random.Range(0, dialog._altInputs.Length)}";

                            return inputKey;
                        }
                        
                        var dialogTrigger = __instance._currentDialogTrigger;
                        if (dialogTrigger && dialogTrigger._useLocalDialogBranch)
                        {
                            return Localyssation.GetString(GetInputKey(KeyUtil.Normalize($"LOCAL_BRANCH_{dialogTrigger.gameObject.scene.name}_{Util.GetChildTransformPath(dialogTrigger.transform, 2)}")), oldString);
                        }
                        else
                        {
                            var branchTypes = new Dictionary<DialogBranch[], string>()
                            {
                                { __instance._scriptableDialog._dialogBranches, "BRANCH" },
                                { __instance._scriptableDialog._introductionBranches, "INTRODUCTION_BRANCH" }
                            };
                            foreach (var branchType in branchTypes)
                            {
                                var branchArray = branchType.Key;
                                var branchTypeName = branchType.Value;
                                if (branchArray.Contains(dialogBranch))
                                {
                                    var branchIndex = Array.IndexOf(branchArray, dialogBranch);
                                    return Localyssation.GetString(GetInputKey($"{branchTypeName}_{branchIndex}"), oldString);
                                }
                            }
                        }
                    }
                    return oldString;
                }));

            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(DialogManager), nameof(DialogManager.Display_NextSentence))]
        [HarmonyPostfix]
        public static void DialogManager_Display_NextSentence(DialogManager __instance)
        {
            if (!__instance._scriptableDialog) return;

            var key = KeyUtil.GetForAsset(__instance._scriptableDialog);

            if (__instance._characterNameText.IsActive())
                __instance._characterNameText.text = Localyssation.GetString($"{key}_NAME_TAG", __instance._characterNameText.text, __instance._characterNameText.fontSize);
        }

        [HarmonyPatch(typeof(DialogManager), nameof(DialogManager.Create_DialogSelectionButton))]
        [HarmonyPostfix]
        public static void DialogManager_Create_DialogSelectionButton(DialogManager __instance, DialogSelection _dialogSelection)
        {
            if (__instance._selectionButtons.Count <= 0 || !__instance._scriptableDialog) return;

            var key = KeyUtil.GetForAsset(__instance._scriptableDialog);

            var newButton = __instance._selectionButtons[__instance._selectionButtons.Count - 1];
            var buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText)
            {
                var branchTypes = new Dictionary<DialogBranch[], string>()
                {
                    { __instance._scriptableDialog._dialogBranches, "BRANCH" },
                    { __instance._scriptableDialog._introductionBranches, "INTRODUCTION_BRANCH" }
                };
                foreach (var branchType in branchTypes)
                {
                    var branchArray = branchType.Key;
                    var branchTypeName = branchType.Value;
                    for (var branchIndex = 0; branchIndex < branchArray.Length; branchIndex++)
                    {
                        var branch = branchArray[branchIndex];
                        for (var dialogIndex = 0; dialogIndex < branch.dialogs.Length; dialogIndex++)
                        {
                            var dialog = branch.dialogs[dialogIndex];
                            for (var selectionIndex = 0; selectionIndex < dialog._dialogSelections.Length; selectionIndex++)
                            {
                                var selection = dialog._dialogSelections[selectionIndex];
                                if (selection == _dialogSelection)
                                {
                                    buttonText.text = Localyssation.GetString($"{key}_{branchTypeName}_{branchIndex}_DIALOG_{dialogIndex}_SELECTION_{selectionIndex}", buttonText.text, buttonText.fontSize);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static Dictionary<string, string> dialogManagerQuickSentencesHack = new Dictionary<string, string>();
        [HarmonyPatch(typeof(DialogManager), nameof(DialogManager.Start_QuickSentence))]
        [HarmonyPostfix]
        public static void DialogManager_Start_QuickSentence(DialogManager __instance, ref string _sentence)
        {
            // most likely shouldn't do this, because two different dialogue entries might share the same string
            // let's hope that this never actually happens

            if (dialogManagerQuickSentencesHack.TryGetValue(_sentence, out var key))
            {
                _sentence = Localyssation.GetString(key, _sentence);
            }
        }
    }

    [HarmonyPatch]
    class PlayerQuestingPatch_Apply_QuestItemProgress
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(PlayerQuesting))
                .Where(methodInfo => methodInfo.Name.Contains($"<{nameof(PlayerQuesting.Apply_QuestItemProgress)}>g__"))
                .Cast<MethodBase>()
                .FirstOrDefault();
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Newarr),
                    new CodeMatch(x => x.IsStloc()));

            var acquiredItemsArray_pos = ReplaceTextPatches.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableQuest), nameof(ScriptableQuest._questObjective))),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestObjective), nameof(QuestObjective._questItemRequirements))),
                new CodeMatch(),
                new CodeMatch(),
                new CodeMatch(x => x.IsStloc()));

            var questItemRequirement_pos = ReplaceTextPatches.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(PlayerQuesting), nameof(PlayerQuesting.Apply_QuestProgressNote))));

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldloc, questItemRequirement_pos),
                new CodeInstruction(OpCodes.Ldloc, acquiredItemsArray_pos),
                Transpilers.EmitDelegate<Func<string, PlayerQuesting, ScriptableQuest, int, QuestItemRequirement, int[], string>>((oldString, __instance, quest, questIndex, questItemRequirement, acquiredItemsArray) =>
                {
                    var questItemRequirementIndex = Array.IndexOf(quest._questObjective._questItemRequirements, questItemRequirement);
                    return string.Format(
                        Localyssation.GetString(
                            "FORMAT_QUEST_PROGRESS",
                            Localyssation.GetString($"{KeyUtil.GetForAsset(questItemRequirement._questItem)}_NAME")),
                        acquiredItemsArray[questItemRequirementIndex],
                        questItemRequirement._itemsNeeded);
                }));

            return matcher.InstructionEnumeration();
        }
    }

    [HarmonyPatch]
    class PlayerQuestingPatch_Apply_QuestTriggerProgress
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(PlayerQuesting))
                .Where(methodInfo => methodInfo.Name.Contains($"<{nameof(PlayerQuesting.Apply_QuestTriggerProgress)}>g__"))
                .Cast<MethodBase>()
                .FirstOrDefault();
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableQuest), nameof(ScriptableQuest._questObjective))),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestObjective), nameof(QuestObjective._questTriggerRequirements))),
                    new CodeMatch(),
                    new CodeMatch(),
                    new CodeMatch(x => x.IsStloc()));

            var questTriggerRequirement_pos = ReplaceTextPatches.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(PlayerQuesting), nameof(PlayerQuesting.Apply_QuestProgressNote))));

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldloc, questTriggerRequirement_pos),
                Transpilers.EmitDelegate<Func<string, PlayerQuesting, ScriptableQuest, int, QuestTriggerRequirement, string>>((oldString, __instance, quest, questIndex, questTriggerRequirement) =>
                {
                    var questTriggerRequirementIndex = Array.IndexOf(quest._questObjective._questTriggerRequirements, questTriggerRequirement);
                    return string.Format(
                        Localyssation.GetString(
                            "FORMAT_QUEST_PROGRESS",
                            $"{questTriggerRequirement._prefix} {questTriggerRequirement._suffix}"),
                        __instance._questProgressData[questIndex]._triggerProgressValues[questTriggerRequirementIndex],
                        questTriggerRequirement._triggerEmitsNeeded);
                }));

            return matcher.InstructionEnumeration();
        }
    }

    [HarmonyPatch]
    class PlayerQuestingPatch_Target_Query_CreepKillProgress
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(PlayerQuesting))
                .Where(methodInfo => methodInfo.Name.Contains($"<{nameof(PlayerQuesting.Target_Query_CreepKillProgress)}>g__"))
                .Cast<MethodBase>()
                .FirstOrDefault();
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableQuest), nameof(ScriptableQuest._questObjective))),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestObjective), nameof(QuestObjective._questCreepRequirements))),
                    new CodeMatch(),
                    new CodeMatch(),
                    new CodeMatch(x => x.IsStloc()));

            var questCreepRequirement_pos = ReplaceTextPatches.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(PlayerQuesting), nameof(PlayerQuesting.Apply_QuestProgressNote))));

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldloc, questCreepRequirement_pos),
                Transpilers.EmitDelegate<Func<string, PlayerQuesting, ScriptableQuest, int, QuestCreepRequirement, string>>((oldString, __instance, quest, questIndex, questCreepRequirement) =>
                {
                    var questCreepRequirementIndex = Array.IndexOf(quest._questObjective._questCreepRequirements, questCreepRequirement);
                    return string.Format(
                        Localyssation.GetString(
                            "FORMAT_QUEST_PROGRESS",
                            ReplaceTextPatches.GetCreepKillRequirementText(questCreepRequirement._questCreep, questCreepRequirement._creepsKilled)),
                        Math.Min(__instance._questProgressData[questIndex]._creepKillProgressValues[questCreepRequirementIndex] + 1, questCreepRequirement._creepsKilled),
                        questCreepRequirement._creepsKilled);
                }));

            return matcher.InstructionEnumeration();
        }
    }
}