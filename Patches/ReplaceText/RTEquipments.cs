﻿using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Localyssation.Patches.ReplaceText
{
    internal static class RTEquipments
    {
        // equipment
        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Apply_EquipStats))]
        [HarmonyPostfix]
        public static void EquipToolTip_Apply_EquipStats(EquipToolTip __instance, ScriptableEquipment _scriptEquip, ItemData _itemData)
        {
            if (_scriptEquip && !__instance._isGambleItem)
            {
                var key = KeyUtil.GetForAsset(_scriptEquip);

                var shownRarity = _scriptEquip._itemRarity;
                if (_itemData._modifierID != 0 && GameManager._current.Locate_StatModifier(_itemData._modifierID))
                {
                    shownRarity += 1;
                }

                if (!string.IsNullOrEmpty(_scriptEquip._itemName))
                    __instance._toolTipName.text = __instance._toolTipName.text.Replace(_scriptEquip._itemName, Localyssation.GetString($"{key}_NAME", __instance._toolTipName.text, __instance._toolTipName.fontSize));
                __instance._toolTipSubName.text = string.Format(
                    Localyssation.GetString("FORMAT_ITEM_RARITY", __instance._toolTipSubName.text, __instance._toolTipSubName.fontSize),
                    Localyssation.GetString(KeyUtil.GetForAsset(shownRarity), _scriptEquip._itemRarity.ToString(), __instance._toolTipSubName.fontSize));

                if (!string.IsNullOrEmpty(_scriptEquip._itemDescription))
                    __instance._toolTipDescription.text = Localyssation.GetString($"{key}_DESCRIPTION", __instance._toolTipDescription.text, __instance._toolTipDescription.fontSize);

                if (_scriptEquip._classRequirement)
                    __instance._equipClassRequirement.text = string.Format(
                        Localyssation.GetString("FORMAT_EQUIP_CLASS_REQUIREMENT", __instance._equipClassRequirement.text, __instance._equipClassRequirement.fontSize),
                        Localyssation.GetString($"{KeyUtil.GetForAsset(_scriptEquip._classRequirement)}_NAME", __instance._equipClassRequirement.text, __instance._equipClassRequirement.fontSize));

                if (_scriptEquip.GetType() == typeof(ScriptableWeapon))
                {
                    var weapon = (ScriptableWeapon)_scriptEquip;

                    if (weapon._weaponConditionSlot._scriptableCondition)
                    {
                        __instance._toolTipDescription.text += string.Format(
                            Localyssation.GetString("FORMAT_EQUIP_WEAPON_CONDITION", __instance._toolTipDescription.text, __instance._toolTipDescription.fontSize),
                            weapon._weaponConditionSlot._chance * 100f,
                            Localyssation.GetString(
                                $"{KeyUtil.GetForAsset(weapon._weaponConditionSlot._scriptableCondition)}_NAME",
                                weapon._weaponConditionSlot._scriptableCondition._conditionName, __instance._toolTipDescription.fontSize)
                            );
                    }

                    if (Enum.TryParse<DamageType>(__instance._weaponTypeText.text, out var damageType))
                        __instance._weaponTypeText.text = __instance._weaponTypeText.text.Replace(
                            damageType.ToString(), Localyssation.GetString(KeyUtil.GetForAsset(damageType),
                            damageType.ToString(), __instance._weaponTypeText.fontSize)
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
                            );
                    }
                    else
                    {
                        __instance._equipStatsDisplay.text = __instance._equipStatsDisplay.text.Replace(
                            "Normal", Localyssation.GetString("COMBAT_ELEMENT_NORMAL_NAME", "Normal", __instance._equipStatsDisplay.fontSize));
                    }
                }
            }
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
                { "<color=#efcc00>({0} - {1})</color> Damage", "FORMAT_EQUIP_STATS_DAMAGE_SCALED_POWERFUL" },
                { "<color=#c5e384>({0} - {1})</color> Damage", "FORMAT_EQUIP_STATS_DAMAGE_SCALED" },
                { "\n<color=grey>(Base Damage: {0} - {1})</color>", "FORMAT_EQUIP_STATS_DAMAGE_COMPARE_BASE" },
                { "({0} - {1}) Damage", "FORMAT_EQUIP_STATS_DAMAGE_UNSCALED" },
                { "Block threshold: {0} damage", "FORMAT_EQUIP_STATS_BLOCK_THRESHOLD" }
            });
        }

    }
}
