using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;
using System.Linq;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(EnchanterManager), nameof(EnchanterManager.Awake))]
        [HarmonyPostfix]
        public static void EnchanterManager_Awake_Postfix(EnchanterManager __instance)
        {
            string controllerButton(string name)
            {
                return $"Canvas_DialogSystem/_dolly_enchanterBox/_backdrop_enchantItem/_button_{name}/_button_{name}Text";
            }
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>()
            {
                { "Canvas_DialogSystem/_dolly_enchanterBox/_backdrop_header/_text_header", I18nKeys.Enchanter.HEADER },
                { controllerButton("clearEnchant"), I18nKeys.Enchanter.BUTTON_CLEAR_SELECTION }
            });
        }

        [HarmonyPatch(typeof(EnchanterManager), nameof(EnchanterManager.Handle_EnchanterBehavior))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EnchanterManager_Handle_EnchanterBehavior_Transpiler
            (IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new string[] {
                    I18nKeys.Enchanter.BUTTON_ENCHANT_ENCHANT,
                    I18nKeys.Enchanter.BUTTON_ENCHANT_REROLL,
                    I18nKeys.Enchanter.BUTTON_ENCHANT_UNABLE,
                    I18nKeys.Enchanter.STATUS_NO_ENCHANT,
                    I18nKeys.Enchanter.STATUS_UNABLE_TO_ENCHANT,
                    I18nKeys.Enchanter.BUTTON_ENCHANT_INSERT_ITEM
                }
                .Concat(I18nKeys.Enchanter.BUTTON_TRANSMUTE), allowRepeat: true
            );
        }

        [HarmonyPatch(typeof(EnchanterManager), nameof(EnchanterManager.Handle_EnchanterBehavior))]
        [HarmonyPostfix]
        public static void EnchanterManager_Handle_EnchanterBehavior_Postfix
            (EnchanterManager __instance)
        {
            if (__instance._isOpen && (bool)__instance._scriptEquipment)
            {
                var _scriptEquipment = __instance._scriptEquipment;
                bool _hasTradeItemCost = (bool)_scriptEquipment && _scriptEquipment._statModifierCost != null && (bool)_scriptEquipment._statModifierCost._scriptItem;
                if (!((int)_scriptEquipment._itemRarity >= 2 || !_scriptEquipment._statModifierTable))    // copied from decompiled source, leave it to compiler for the optimization
                {
                    //_currency
                    //Text.text = $"{_setEnchantPrice} {GameManager._current._statLogics._currencyName}";
                    int _setEnchantPrice = _scriptEquipment._vendorCost * 3; // min at 3 so plural form is confirmed
                    __instance._currencyPriceText.text = $"{_setEnchantPrice} {Localyssation.GetString(I18nKeys.Lore.CROWN_PLURAL)}";
                    if (_hasTradeItemCost)
                    {
                        //int _foundTradeQuantity = 0;
                        //for (int j = 0; j < Player._mainPlayer._pInventory._heldItems.Count; j++)
                        //{
                        //    if (Player._mainPlayer._pInventory._heldItems[j]._itemName == _scriptEquipment._statModifierCost._scriptItem._itemName)
                        //    {
                        //        _foundTradeQuantity += Player._mainPlayer._pInventory._heldItems[j]._quantity;
                        //    }
                        //}
                        int _foundTradeQuantity = Player._mainPlayer._pInventory._heldItems
                            .Where(item => item._itemName == _scriptEquipment._statModifierCost._scriptItem._itemName)
                            .Aggregate(0, (sum, itemdata) => sum + itemdata._quantity);

                        __instance._tradeItemPriceText.text = $"{_foundTradeQuantity}/{_scriptEquipment._statModifierCost._scriptItemQuantity} "
                            + Localyssation.GetString(KeyUtil.GetForAsset(_scriptEquipment._statModifierCost._scriptItem) + "_NAME");
                    }
                }
                if (__instance._setItemData._modifierID > 0)
                {
                    ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(__instance._setItemData._modifierID);
                    __instance._currentEnchantmentText.text = Localyssation.GetString(I18nKeys.Enchanter.STATUS_CURRENT_ENCHANTMENT)
                        + Localyssation.GetString(KeyUtil.GetForAsset(scriptableStatModifier) + "_TAG");
                }
            }
        }
    }
}
