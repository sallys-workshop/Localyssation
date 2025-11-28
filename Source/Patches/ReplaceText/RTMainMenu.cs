using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {

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
                        RTUtil.RemapAllTextUnderObject(obj_dolly_selectBar.gameObject, new Dictionary<string, string>()
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
                                            var newTrigger = new EventTrigger.Entry
                                            {
                                                eventID = trigger.eventID
                                            };
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
                    var obj_menuBackdrop = parent.Find("_gameStartMenu/Canvas_gameStart/_dolly_multiplayerMenu/_menuBackdrop");
                    if (obj_menuBackdrop != null)
                    {
                        RTUtil.RemapAllTextUnderObject(obj_menuBackdrop.gameObject, new Dictionary<string, string>() {
                            { "_button_joinServer", I18nKeys.MainMenu.BUTTON_JOIN_SERVER },
                            { "_button_hostServer", I18nKeys.MainMenu.BUTTON_HOST_SERVER },
                            { "_button_return", I18nKeys.MainMenu.BUTTON_RETURN }

                        });
                    }
                }

                var obj_Canvas_characterSelect = parent.Find("_characterSelectMenu/Canvas_characterSelect");
                if (obj_Canvas_characterSelect)
                {
                    RTUtil.RemapAllTextUnderObject(obj_Canvas_characterSelect.gameObject, new Dictionary<string, string>()
                    {
                        { "_text_header", "CHARACTER_SELECT_HEADER" },
                        //{ "_button_createCharacter", "CHARACTER_SELECT_BUTTON_CREATE_CHARACTER" },
                        { "_button_deleteCharacter", "CHARACTER_SELECT_BUTTON_DELETE_CHARACTER" },
                        { "_button_select", "CHARACTER_SELECT_BUTTON_SELECT_CHARACTER" },
                        { "_button_return", "CHARACTER_SELECT_BUTTON_RETURN" },
                        { "_text_characterDeletePrompt", "CHARACTER_SELECT_CHARACTER_DELETE_PROMPT_TEXT" },
                        { "_button_confirmDeleteCharacter", "CHARACTER_SELECT_CHARACTER_DELETE_BUTTON_CONFIRM" },
                        { "_button_deletePrompt_return", "CHARACTER_SELECT_CHARACTER_DELETE_BUTTON_RETURN" },
                    });

                    RTUtil.RemapAllInputPlaceholderTextUnderObject(obj_Canvas_characterSelect.gameObject, new Dictionary<string, string>()
                    {
                        { "_input_characterDeleteConfirm", "CHARACTER_SELECT_CHARACTER_DELETE_PROMPT_PLACEHOLDER_TEXT" }
                    });
                }

                var obj_Canvas_characterCreation = parent.Find("_characterSelectMenu/Canvas_characterCreation");
                if (obj_Canvas_characterCreation)
                {
                    RTUtil.RemapAllTextUnderObject(obj_Canvas_characterCreation.gameObject, new Dictionary<string, string>()
                    {
                        { "_text_header", "CHARACTER_CREATION_HEADER" },
                        { "_header_raceSelect", "CHARACTER_CREATION_HEADER" },
                        { "_header_initialSkill", "CHARACTER_CREATION_RACE_DESCRIPTOR_HEADER_INITIAL_SKILL" },
                        { "_button_createCharacter", "CHARACTER_CREATION_BUTTON_CREATE_CHARACTER" },
                        { "_button_return", "CHARACTER_CREATION_BUTTON_RETURN" },
                    });

                    RTUtil.RemapAllInputPlaceholderTextUnderObject(obj_Canvas_characterSelect.gameObject, new Dictionary<string, string>()
                    {
                        { "_input_characterName", "CHARACTER_CREATION_CHARACTER_NAME_PLACEHOLDER_TEXT" }
                    });

                    var customizer_color = obj_Canvas_characterCreation.transform.Find("_dolly_customizer/_customizer_color");
                    var customizer_head = obj_Canvas_characterCreation.transform.Find("_dolly_customizer/_customizer_head");
                    var customizer_body = obj_Canvas_characterCreation.transform.Find("_dolly_customizer/_customizer_body");
                    var customizer_trait = obj_Canvas_characterCreation.transform.Find("_dolly_customizer/_customizer_trait");
                    if (customizer_color)
                        RTUtil.RemapAllTextUnderObject(customizer_color.gameObject, new Dictionary<string, string>()
                        {
                            { "_customizer_header", "CHARACTER_CREATION_CUSTOMIZER_HEADER_COLOR" },
                            { "_text_bodyColor", "CHARACTER_CREATION_CUSTOMIZER_COLOR_BODY_HEADER" },
                            { "_characterButtonSelector", "CHARACTER_CREATION_CUSTOMIZER_COLOR_BODY_TEXTURE" },
                            { "_text_hairColor", "CHARACTER_CREATION_CUSTOMIZER_COLOR_HAIR_HEADER" },
                            { "Toggle_lockColor", "CHARACTER_CREATION_CUSTOMIZER_COLOR_HAIR_LOCK_COLOR" },
                            //{ "_button_defaultColor", "CHARACTER_CREATION_BUTTON_SET_TO_DEFAULTS" },
                        });
                    if (customizer_head)
                        RTUtil.RemapAllTextUnderObject(customizer_head.gameObject, new Dictionary<string, string>()
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
                        RTUtil.RemapAllTextUnderObject(customizer_body.gameObject, new Dictionary<string, string>()
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
                            //{ "_button_defaultBody", "CHARACTER_CREATION_BUTTON_SET_TO_DEFAULTS" },
                        });
                    if (customizer_trait)
                        RTUtil.RemapAllTextUnderObject(customizer_trait.gameObject, new Dictionary<string, string>()
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
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "Singleplayer", "CHARACTER_SELECT_HEADER_GAME_MODE_SINGLEPLAYER" },
                { "Host Game (Public)", "CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_PUBLIC" },
                { "Host Game (Friends)", "CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_FRIENDS" },
                { "Host Game (Private)", "CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_PRIVATE" },
                { "Join Game", "CHARACTER_SELECT_HEADER_GAME_MODE_JOIN_MULTIPLAYER" },
                { "Lobby Connect", "CHARACTER_SELECT_HEADER_GAME_MODE_LOBBY_QUERY" },
            });
        }
        [HarmonyPatch(typeof(CharacterSelectManager), nameof(CharacterSelectManager.Update))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> CharacterSelectManager_Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new[] {
                I18nKeys.MainMenu.PAGER
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

                string raceName = "";
                string className = Localyssation.GetString("PLAYER_CLASS_EMPTY_NAME", GameManager._current._statLogics._emptyClassName);

                ScriptablePlayerRace race = GameManager._current.Locate_PlayerRace(__instance._characterFileData._appearanceProfile._setRaceTag);
                if (race) 
                    raceName = KeyUtil.GetForAsset(race).Name.Localize();

                
                if (!string.IsNullOrEmpty(__instance._characterFileData._statsProfile._classID))
                {
                    PlayerStats_Profile statsProfile = __instance._characterFileData._statsProfile;
                    ScriptablePlayerBaseClass playerClass = GameManager._current.Locate_PlayerClass(statsProfile._classID);
                    if (playerClass)
                    {
                        className = KeyUtil.GetForAsset(playerClass).Name.Localize();
                        if (statsProfile._classTier > 0)
                        {
                            className = KeyUtil.GetForAsset(playerClass._playerClassTiers[statsProfile._classTier - 1]).Name.Localize();
                        }
                    }
                        
                }

                __instance._characterInfoText.text = string.Format(Localyssation.GetString(
                    I18nKeys.CharacterSelect.FORMAT_DATA_ENTRY_INFO,
                    __instance._characterInfoText.text),
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

                __instance._raceDescriptionHeader.text = Localyssation.GetString($"{key}_NAME", __instance._raceDescriptionHeader.text, __instance._raceDescriptionHeader.fontSize) ?? "";
                __instance._raceDescriptorField.text = Localyssation.GetString($"{key}_DESCRIPTION", __instance._raceDescriptorField.text, __instance._raceDescriptorField.fontSize) ?? "";
                __instance._colorMiscTag.text = Localyssation.GetString($"{key}_MISC", __instance._colorMiscTag.text, __instance._colorMiscTag.fontSize) ?? "";
                __instance._miscTag.text = Localyssation.GetString($"{key}_MISC", __instance._miscTag.text, __instance._miscTag.fontSize) ?? "";

                var skillKey = KeyUtil.GetForAsset(race._racialSkill);
                __instance._raceInitialSkillTag.text = Localyssation.GetString($"{skillKey}_NAME", __instance._raceInitialSkillTag.text, __instance._raceInitialSkillTag.fontSize) ?? "";
                __instance._raceInitialSkillDescriptor.text = Localyssation.GetString($"{skillKey}_DESCRIPTION", __instance._raceInitialSkillDescriptor.text, __instance._raceInitialSkillDescriptor.fontSize) ?? "";
            }
        }
    }

    [HarmonyPatch]
    class CharacterSelectManager__Handle_CharacterSelectControl__Transpiler
    {
        private static readonly TargetInnerMethod TARGET = new TargetInnerMethod() { 
            Type = typeof(CharacterSelectManager), 
            ParentMethodName = nameof(CharacterSelectManager.Handle_CharacterSelectControl) ,
            InnerMethodName = "Handle_ButtonControl"
        };
        static MethodBase TargetMethod() => TranspilerHelper.GenerateTargetMethod(TARGET);

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new List<string>() {
                I18nKeys.CharacterSelect.BUTTON_SELECT_CHARACTER,
                I18nKeys.CharacterSelect.BUTTON_CREATE_CHARACTER
            });
        }
    }
}
