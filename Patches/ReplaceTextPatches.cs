using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Localyssation.Patches
{
    internal static class ReplaceTextPatches
    {
        private static IEnumerable<CodeInstruction> SimpleStringReplaceTranspiler(IEnumerable<CodeInstruction> instructions, Dictionary<string, string> stringReplacements)
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
                        return Localyssation.GetString(stringReplacements[key]);
                    }));
                    replacedStrings.Add(key);
                }).InstructionEnumeration();
        }

        public static void RemapAllTextUnderObject(GameObject gameObject, Dictionary<string, string> textRemaps, Action<Transform, string> onRemap = null)
        {
            bool TryRemapSingle(Transform lookupNameTransform, Text text)
            {
                if (textRemaps.TryGetValue(lookupNameTransform.name, out var key))
                {
                    LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc(key));
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

        public static void RemapAllInputPlaceholderTextUnderObject(GameObject gameObject, Dictionary<string, string> textRemaps, Action<Transform, string> onRemap = null)
        {
            foreach (var inputField in gameObject.GetComponentsInChildren<InputField>())
            {
                if (inputField.placeholder)
                {
                    var text = inputField.placeholder.GetComponent<Text>();
                    if (text && textRemaps.TryGetValue(inputField.name, out var key))
                    {
                        LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc(key));
                        if (onRemap != null) onRemap(inputField.transform, key);
                    }
                }
            }
        }

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
                                                tooltipText.text = Localyssation.GetString($"{key}_TOOLTIP", tooltipText.fontSize);
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
            }
        }

        [HarmonyPatch(typeof(CharacterSelectManager), nameof(CharacterSelectManager.Handle_HeaderText))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> CharacterSelectManager_Handle_HeaderText(IEnumerable<CodeInstruction> instructions)
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
                __instance._characterNicknameText.text = Localyssation.GetString("CHARACTER_SELECT_DATA_ENTRY_EMPTY_SLOT");
            }
            else
            {
                var raceName = "";
                var className = Localyssation.GetString("PLAYER_CLASS_EMPTY_NAME");

                var race = GameManager._current.LocateRace(__instance._characterFileData._appearanceProfile._setRaceTag);
                if (race) raceName = Localyssation.GetString($"{KeyUtil.GetForAsset(race)}_NAME");

                if (!string.IsNullOrEmpty(__instance._characterFileData._statsProfile._classID))
                {
                    var playerClass = GameManager._current.LocateClass(__instance._characterFileData._statsProfile._classID);
                    if (playerClass) className = Localyssation.GetString($"{KeyUtil.GetForAsset(playerClass)}_NAME");
                }

                __instance._characterInfoText.text = Localyssation.GetFormattedString(
                    "FORMAT_CHARACTER_SELECT_DATA_ENTRY_INFO",
                    __instance._characterInfoText.fontSize,
                    __instance._characterFileData._statsProfile._currentLevel,
                    raceName,
                    className);
            }
        }

        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Start))]
        [HarmonyPostfix]
        public static void SettingsManager_Start(SettingsManager __instance)
        {
            RemapAllTextUnderObject(__instance.gameObject, new Dictionary<string, string>()
            {
                { "Button_videoTab", "SETTINGS_TAB_BUTTON_VIDEO" },

                { "_header_GameEffectSettings", "SETTINGS_VIDEO_HEADER_GAME_EFFECT_SETTINGS" },
                { "_cell_proportionsToggle", "Limit Player Character Proportions" },
                { "_cell_jiggleBonesToggle", "Disable Suggestive Jiggle Bones" },
                { "_cell_clearUnderclothesToggle", "Enable Clear Clothing" },

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
                { "Button_01", "SETTINGS_BUTTON_RESET" },
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
                { "_cell_displayLocalNametag", "SETTINGS_NETWORK_CELL_DISPLAY_LOCAL_NAMETAG" },
                { "_cell_displayHostTag", "SETTINGS_NETWORK_CELL_DISPLAY_HOST_TAG" },
                { "_cell_hideDungeonMinimap", "SETTINGS_NETWORK_CELL_HIDE_DUNGEON_MINIMAP" },
                { "_cell_hideFPSCounter", "SETTINGS_NETWORK_CELL_HIDE_FPS_COUNTER" },
                { "_cell_hidePingCounter", "SETTINGS_NETWORK_CELL_HIDE_PING_COUNTER" },

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
                            dropdownOptionsTextFuncs.Add(LangAdjustables.GetStringFunc(dropdownOptionKey));
                    }

                    if (dropdownOptionsTextFuncs.Count == dropdown.options.Count)
                    {
                        LangAdjustables.RegisterDropdown(dropdown, dropdownOptionsTextFuncs);
                    }
                }
            });
        }

        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Apply_EquipStats))]
        [HarmonyPostfix]
        public static void EquipToolTip_Apply_EquipStats(EquipToolTip __instance, ScriptableEquipment _scriptEquip, ItemData _itemData)
        {
            if (_scriptEquip)
            {
                var key = KeyUtil.GetForAsset(_scriptEquip);

                if (!string.IsNullOrEmpty(_scriptEquip._itemName))
                    __instance._toolTipName.text = __instance._toolTipName.text.Replace(_scriptEquip._itemName, Localyssation.GetString($"{key}_NAME"));
                __instance._toolTipSubName.text = Localyssation.GetFormattedString(
                    "FORMAT_EQUIP_ITEM_RARITY",
                    __instance._toolTipSubName.fontSize,
                    Localyssation.GetString(KeyUtil.GetForAsset(_scriptEquip._itemRarity)));

                if (!string.IsNullOrEmpty(_scriptEquip._itemDescription))
                    __instance._toolTipDescription.text = __instance._toolTipDescription.text.Replace(_scriptEquip._itemDescription, Localyssation.GetString($"{key}_DESCRIPTION"));

                if (_scriptEquip._classRequirement)
                    __instance._equipClassRequirement.text = Localyssation.GetFormattedString(
                        "FORMAT_EQUIP_CLASS_REQUIREMENT",
                        __instance._equipClassRequirement.fontSize,
                        Localyssation.GetString($"{KeyUtil.GetForAsset(_scriptEquip._classRequirement)}_NAME"));

                if (_scriptEquip.GetType() == typeof(ScriptableWeapon))
                {
                    var weapon = (ScriptableWeapon)_scriptEquip;

                    if (Enum.TryParse<DamageType>(__instance._equipWeaponDamageType.text, out var damageType))
                        __instance._equipWeaponDamageType.text = __instance._equipWeaponDamageType.text.Replace(damageType.ToString(), Localyssation.GetString(KeyUtil.GetForAsset(damageType)));

                    if (weapon._combatElement)
                    {
                        if (!string.IsNullOrEmpty(weapon._combatElement._elementName))
                            __instance._equipElementText.text = __instance._equipElementText.text.Replace(weapon._combatElement._elementName, Localyssation.GetString($"{KeyUtil.GetForAsset(weapon._combatElement)}_NAME"));
                    }
                    else
                    {
                        __instance._equipElementText.text = __instance._equipElementText.text.Replace("Normal", Localyssation.GetString("COMBAT_ELEMENT_NORMAL_NAME"));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Apply_EquipStats))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EquipToolTip_Apply_EquipStats_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
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

        [HarmonyPatch(typeof(QuestTrackElement), nameof(QuestTrackElement.Handle_QuestTrackInfo))]
        [HarmonyPostfix]
        public static void QuestTrackElement_Handle_QuestTrackInfo(QuestTrackElement __instance)
        {
            var key = KeyUtil.GetForAsset(__instance._scriptQuest);
            if (!string.IsNullOrEmpty(__instance._scriptQuest._questName))
                __instance._trackQuestNameText.text = __instance._trackQuestNameText.text.Replace(__instance._scriptQuest._questName, Localyssation.GetString($"{key}_NAME"));

            var playerQuesting = Player._mainPlayer.GetComponent<PlayerQuesting>();
            if (playerQuesting._questProgressData.Count > 0)
            {
                var questProgressData = playerQuesting._questProgressData[__instance._questIndex];

                var trackElementText = __instance._trackElementText.text.Split(new string[] { "\n" }, StringSplitOptions.None);
                var c = 0;
                void ReplaceTrackElementText(string newText, int progressCurrent, int progressMax)
                {
                    var styleTag = trackElementText[c].Substring(0, trackElementText[c].IndexOf(">") + 1);
                    var formattedQuestString = Localyssation.GetFormattedString(
                        "FORMAT_QUEST_TRACK_ELEMENT",
                        __instance._trackElementText.fontSize,
                        newText, progressCurrent, progressMax);
                    trackElementText[c] = styleTag + formattedQuestString + "</color>";
                    c++;
                }

                for (var i = 0; i < __instance._scriptQuest._questObjective._questItemRequirements.Length; i++)
                {
                    var questItemRequirement = __instance._scriptQuest._questObjective._questItemRequirements[i];
                    var itemKey = $"{KeyUtil.GetForAsset(questItemRequirement._questItem)}_NAME";
                    ReplaceTrackElementText(Localyssation.GetString(itemKey), questProgressData._itemProgressValues[i], questItemRequirement._itemsNeeded);
                }
                for (var i = 0; i < __instance._scriptQuest._questObjective._questCreepRequirements.Length; i++)
                {
                    var questCreepRequirement = __instance._scriptQuest._questObjective._questCreepRequirements[i];
                    var creepKey = $"{KeyUtil.GetForAsset(questCreepRequirement._questCreep)}_NAME_IN_QUEST_TRACK_SLAIN{(questCreepRequirement._creepsKilled <= 1 ? "" : "_MULTIPLE")}";
                    ReplaceTrackElementText(Localyssation.GetString(creepKey), questProgressData._creepKillProgressValues[i], questCreepRequirement._creepsKilled);
                }
                for (var i = 0; i < __instance._scriptQuest._questObjective._questTriggerRequirements.Length; i++)
                {
                    var questTriggerRequirement = __instance._scriptQuest._questObjective._questTriggerRequirements[i];
                    var triggerKey = KeyUtil.GetForAsset(questTriggerRequirement);
                    ReplaceTrackElementText($"{Localyssation.GetString(triggerKey + "_PREFIX")} {Localyssation.GetString(triggerKey + "_SUFFIX")}", questProgressData._triggerProgressValues[i], questTriggerRequirement._triggerEmitsNeeded);
                }

                __instance._trackElementText.text = string.Join("\n", trackElementText);
            }
        }
    }
}