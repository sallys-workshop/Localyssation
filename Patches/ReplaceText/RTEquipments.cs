using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {

        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Awake))]
        [HarmonyPostfix]
        public static void replaceStatTags(EquipToolTip __instance)
        {
            const string TAG_NAME_REGEX = @"_statCell_(\w*)Tag";
            foreach (var tag in __instance.transform.GetComponentsInChildren<Text>(true).Where(text => Regex.IsMatch(text.transform.name, TAG_NAME_REGEX)))
            {
                string statName = Regex.Match(tag.transform.name, TAG_NAME_REGEX).Groups[1].Value;
                string key = I18nKeys.Equipment.statDisplayKey(statName);
                Localyssation.logger.LogDebug($"public static readonly string {key.Replace("ITEM_", "")}\n\t= create(statDisplayKey(\"{statName}\"), \"{tag.text}\");");
                tag.text = Localyssation.GetString(key);
            }
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
                if (!string.IsNullOrEmpty(_scriptEquip._itemName))
                {
                    __instance._toolTipName.text = __instance._toolTipName.text.Replace(_scriptEquip._itemName, Localyssation.GetString($"{key}_NAME", __instance._toolTipName.text, __instance._toolTipName.fontSize));
                }
                if (_itemData._modifierID != 0 && GameManager._current.Locate_StatModifier(_itemData._modifierID))
                {
                    shownRarity += 1;
                    ScriptableStatModifier modifier = GameManager._current.Locate_StatModifier(_itemData._modifierID);
                    __instance._toolTipName.text = __instance._toolTipName.text.Replace( 
                        modifier._modifierTag, Localyssation.GetString(KeyUtil.GetForAsset(modifier) + "_TAG")
                    );
                }
                

                __instance._toolTipSubName.text = string.Format(
                    Localyssation.GetString(I18nKeys.Item.FORMAT_ITEM_RARITY, __instance._toolTipSubName.text, __instance._toolTipSubName.fontSize),
                    Localyssation.GetString(KeyUtil.GetForAsset(shownRarity), _scriptEquip._itemRarity.ToString(), __instance._toolTipSubName.fontSize));

                __instance._toolTipDescription.text = "";
                if (!string.IsNullOrEmpty(_scriptEquip._itemDescription))
                    __instance._toolTipDescription.text = Localyssation.GetString($"{key}_DESCRIPTION", __instance._toolTipDescription.text, __instance._toolTipDescription.fontSize);

                if (_scriptEquip._classRequirement)
                    __instance._equipClassRequirement.text = string.Format(
                        Localyssation.GetString(I18nKeys.Equipment.FORMAT_CLASS_REQUIREMENT, __instance._equipClassRequirement.text, __instance._equipClassRequirement.fontSize),
                        Localyssation.GetString($"{KeyUtil.GetForAsset(_scriptEquip._classRequirement)}_NAME", __instance._equipClassRequirement.text, __instance._equipClassRequirement.fontSize));

                if (_scriptEquip.GetType() == typeof(ScriptableWeapon))
                {
                    var weapon = (ScriptableWeapon)_scriptEquip;

                    if (weapon._weaponConditionSlot._scriptableCondition)
                    {
                        __instance._toolTipDescription.text += string.Format(
                            Localyssation.GetString(I18nKeys.Equipment.FORMAT_WEAPON_CONDITION, __instance._toolTipDescription.text, __instance._toolTipDescription.fontSize),
                            weapon._weaponConditionSlot._chance * 100f,
                            Localyssation.GetString(
                                $"{KeyUtil.GetForAsset(weapon._weaponConditionSlot._scriptableCondition)}_NAME",
                                weapon._weaponConditionSlot._scriptableCondition._conditionName, __instance._toolTipDescription.fontSize)
                            );
                    }
                    DamageType combatType = weapon.weaponType._combatType;
                    __instance._weaponTypeText.text = string.Format(
                        Localyssation.GetString(I18nKeys.Equipment.FORMAT_WEAPON_DAMAGE_TYPE), 
                        Localyssation.GetString(KeyUtil.GetForAsset(combatType))
                    );

                    //_weaponDamageTransmuteText.text = $"Damage Transmute: {_overrideType}"
                    DamageType _overrideType = weapon.weaponType._combatType;
                    PlayerCombat _pCombat = Player._mainPlayer._pCombat;
                    if (_pCombat._useDamageTypeOverride && _itemData._isEquipped && ((_pCombat._isUsingAltWeapon && _itemData._isAltWeapon) || (!_pCombat._isUsingAltWeapon && !_itemData._isAltWeapon)))
                    {
                        _overrideType = _pCombat._damageTypeOverride;
                    }

                    if (_itemData._useDamageTypeOverride)
                    {
                        _overrideType = _itemData._damageTypeOverride;
                    }
                    __instance._weaponDamageTransmuteText.text = string.Format(
                        Localyssation.GetString(I18nKeys.Equipment.FORMAT_WEAPON_TRANSMUTE_TYPE),
                        Localyssation.GetString(KeyUtil.GetForAsset(_overrideType))
                    );
                        

                    //__instance._equipToolTipType.text = $"{weapon.weaponType._weaponAnimSlots[weapon._weaponHoldClipIndex]._weaponNameTag} (Weapon)";
                    __instance._equipToolTipType.text = string.Format(
                        Localyssation.GetString(I18nKeys.Equipment.FORMAT_TOOLTIP_TYPE_WEAPON),
                        Localyssation.GetString(KeyUtil.GetForAsset(weapon.weaponType._weaponAnimSlots[weapon._weaponHoldClipIndex]))
                    );

                    

                    if (weapon._combatElement)
                    {
                        if (!string.IsNullOrEmpty(weapon._combatElement._elementName))
                            __instance._equipStatsDisplay.text = __instance._equipStatsDisplay.text.Replace(
                                weapon._combatElement._elementName,
                                Localyssation.GetString(
                                    $"{KeyUtil.GetForAsset(weapon._combatElement)}_NAME",
                                    weapon._combatElement._elementName,
                                    __instance._equipStatsDisplay.fontSize
                                )
                            )
                                .Replace("Base Damage", Localyssation.GetString(I18nKeys.Equipment.STATS_BASE_DAMAGE))
                                .Replace("Damage", Localyssation.GetString(I18nKeys.Equipment.STATS_DAMAGE));
                    }
                    else
                    {
                        __instance._equipStatsDisplay.text = __instance._equipStatsDisplay.text.Replace(
                            "Normal", Localyssation.GetString(I18nKeys.Lore.COMBAT_ELEMENT_NORMAL_NAME, "Normal", __instance._equipStatsDisplay.fontSize));
                    }
                }
            }
            //return false;
        }

        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Update))]
        [HarmonyPostfix]
        public static void EquipToolTip_Update_Postfix(EquipToolTip __instance)
        {
            __instance._compareDisplayText.text = __instance._compareDisplayText.text.Replace("Compare", Localyssation.GetString(I18nKeys.Equipment.COMPARE));
        }
        
        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Apply_EquipStats))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EquipToolTip_Apply_EquipStats_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
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
                { "<color=#efcc00>({0} - {1})</color> {2}Damage", "FORMAT_EQUIP_STATS_DAMAGE_SCALED_POWERFUL" },
                { "({0} - {1}) Damage", "FORMAT_EQUIP_STATS_DAMAGE_UNSCALED" },
                { "Block threshold: {0} damage", "FORMAT_EQUIP_STATS_BLOCK_THRESHOLD" }
            });
        }

    }
}
