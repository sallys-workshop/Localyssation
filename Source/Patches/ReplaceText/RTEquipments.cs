using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {

        [HarmonyPatch(typeof(TooltipElement), nameof(TooltipElement.Awake))]
        [HarmonyPostfix]
        public static void EquipToolTip__Awake__Postfix(TooltipElement __instance)
        {
            const string TAG_NAME_REGEX = @"_statCell_(\w*)Tag";
            if (__instance is EquipToolTip equipToolTip)
            {
                var nameMap = new Dictionary<string, string>();
                foreach (var statCell in equipToolTip.GetComponentsInChildren<Text>())
                {
                    var match = Regex.Match(statCell.name, TAG_NAME_REGEX);
                    if (match.Success)
                    {
                        var statName = match.Groups[1].Value;
                        var path = $"_statCell_{statName}/_statCell_{statName}Tag";
                        if (Regex.IsMatch(statName, @"resist[A-Z][a-z]*"))
                        {
                            path = $"_statCell_{Regex.Replace(statName, "[A-Z]", s => "_" + s.Value.ToLower())}/_statCell_{statName}Tag";
                        }
                        var key = I18nKeys.Equipment.statDisplayKey(statName);
                        nameMap[path] = key;
                    }
                }
                RTUtil.RemapChildTextsByPath(equipToolTip._attackPowerTag.transform.parent.parent , nameMap);
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
                        modifier._modifierTag, Localyssation.GetString(KeyUtil.GetForAsset(modifier))
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
                        Localyssation.GetString(KeyUtil.GetForAsset(weapon.weaponType))
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
                        __instance._equipStatsDisplay.text = __instance._equipStatsDisplay.text
                            .Replace("Base Damage", Localyssation.GetString(I18nKeys.Equipment.STATS_BASE_DAMAGE))
                            .Replace("Damage", Localyssation.GetString(I18nKeys.Equipment.STATS_DAMAGE));
                    }
                }
            }
            //return false;
        }

        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Update))]
        [HarmonyPostfix]
        public static void EquipToolTip_Update_Postfix(EquipToolTip __instance)
        {
            if (!TabMenu._current || !TabMenu._current._isOpen)
            {
                return;
            }
            if (TabMenu._current._itemTradeMode)
            {
                if ((bool)__instance._specialCurrencyItem)
                {
                    //__instance._vendorValueCounter.text = $"{__instance._vendorValue} {__instance._specialCurrencyItem._itemName}s";
                    __instance._vendorValueCounter.text = $"{__instance._vendorValue} {Localyssation.GetString(KeyUtil.GetForAsset(__instance._specialCurrencyItem) + "_NAME")}";
                }
                else
                {
                    if (__instance._vendorValue > 1)
                    {
                        __instance._vendorValueCounter.text = $"{__instance._vendorValue} {Localyssation.GetString(I18nKeys.Lore.CROWN_PLURAL)}";
                    }
                    else
                    {
                        __instance._vendorValueCounter.text = $"{__instance._vendorValue} {Localyssation.GetString(I18nKeys.Lore.CROWN)}";
                    }
                }
            }
            __instance._compareDisplayText.text = __instance._compareDisplayText.text.Replace("Compare", Localyssation.GetString(I18nKeys.Equipment.COMPARE));
        }

        [HarmonyPatch(typeof(EquipToolTip), nameof(EquipToolTip.Apply_EquipStats))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EquipToolTip_Apply_EquipStats_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new string[] {
                    I18nKeys.Equipment.FORMAT_LEVEL_REQUIREMENT,
                    I18nKeys.Equipment.TOOLTIP_TYPE_HELM,
                    I18nKeys.Equipment.TOOLTIP_TYPE_CHESTPIECE,
                    I18nKeys.Equipment.TOOLTIP_TYPE_LEGGINGS,
                    I18nKeys.Equipment.TOOLTIP_TYPE_CAPE,
                    I18nKeys.Equipment.TOOLTIP_TYPE_RING,
                    //I18nKeys.Equipment.FORMAT_STATS_DAMAGE_UNSCALED,
                    //I18nKeys.Equipment.
                })
                .Unwrap();
        }

    }

    [HarmonyPatch(typeof(EquipToolTip))]
    internal class EquipToolTip__Apply_EquipStats__Display_GambleEquipTooltip
    {
        /// <summary>
        /// Config here when game code change
        /// <para InnerMethodName>Nested method name.</para>
        /// <para ParentMethodName>The method your target nested in.</para>
        /// <para Type>Type where your parent method is declared.</para>
        /// </summary>
        private static readonly TargetInnerMethod __CONDITION = new TargetInnerMethod()
        {
            InnerMethodName = "Display_GambleEquipTooltip",
            ParentMethodName = nameof(EquipToolTip.Apply_EquipStats),
            Type = typeof(EquipToolTip)
        };

        /// Keys to replace, must be registered in <see cref="I18nKeys.TR_KEYS"/>
        public static string[] REPLACEMENT = new string[] {
                I18nKeys.Equipment.TOOLTIP_GAMBLE_ITEM_NAME,
                I18nKeys.Equipment.TOOLTIP_GAMBLE_ITEM_DESCRIPTION,
                I18nKeys.Equipment.TOOLTIP_GAMBLE_ITEM_TYPE,
                I18nKeys.Equipment.TOOLTIP_GAMBLE_ITEM_RARITY,
            };

        public static MethodBase TargetMethod() => TranspilerHelper.GenerateTargetMethod(__CONDITION);
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => RTUtil.SimpleStringReplaceTranspiler(instructions, REPLACEMENT);

    }

    [HarmonyPatch(typeof(EquipToolTip))]
    internal class EquipToolTip__Apply_EquipStats__Init_ShieldToolTip
    {
        private static readonly TargetInnerMethod __CONDITION = new TargetInnerMethod()
        {
            InnerMethodName = "Init_ShieldToolTip",
            ParentMethodName = nameof(EquipToolTip.Apply_EquipStats),
            Type = typeof(EquipToolTip)
        };
        public static readonly string[] REPLACEMENT = new[] {
            I18nKeys.Equipment.TOOLTIP_TYPE_SHIELD
        };

        public static MethodBase TargetMethod() => TranspilerHelper.GenerateTargetMethod(__CONDITION);
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => RTUtil.SimpleStringReplaceTranspiler(instructions, REPLACEMENT);
    }

    //[HarmonyPatch(typeof(EquipToolTip))]
    //internal class EquipToolTip__Apply_EquipStats__Init_WeaponTooltip
    //{
    //    private static readonly TargetInnerMethod __CONDITION = new TargetInnerMethod()
    //    {
    //        InnerMethodName = "Init_WeaponTooltip",
    //        ParentMethodName = nameof(EquipToolTip.Apply_EquipStats),
    //        Type = typeof(EquipToolTip)
    //    };
    //    public static readonly string[] REPLACEMENT = new[] {
    //    };
    //}
}
