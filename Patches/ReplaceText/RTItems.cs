using HarmonyLib;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Localyssation.Patches.ReplaceText
{
    internal static class RTItems
    {
        // items
        [HarmonyPatch(typeof(ItemToolTip), nameof(ItemToolTip.Apply_ItemStats))]
        [HarmonyPostfix]
        public static void ItemToolTip_Apply_ItemStats(ItemToolTip __instance)
        {
            if (__instance._scriptItem)
            {
                if (TabMenu._current._itemTradeMode)
                {
                    if (__instance._setItemQuantity <= 1)
                        __instance._vendorValueCounter.text = string.Format(
                            Localyssation.GetString("FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER", __instance._vendorValueCounter.text, __instance._vendorValueCounter.fontSize),
                            __instance._vendorValue);
                    else
                        __instance._vendorValueCounter.text = string.Format(
                            Localyssation.GetString("FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER_MULTIPLE", __instance._vendorValueCounter.text, __instance._vendorValueCounter.fontSize),
                            __instance._vendorValue,
                            __instance._vendorValue * __instance._setItemQuantity);
                }

                if (__instance._isGambleItem) return;

                var key = KeyUtil.GetForAsset(__instance._scriptItem);

                __instance._toolTipName.text = __instance._toolTipName.text.Replace(__instance._scriptItem._itemName, Localyssation.GetString($"{key}_NAME"));
                __instance._toolTipDescription.text = __instance._toolTipDescription.text.Replace(__instance._scriptItem._itemDescription, Localyssation.GetString($"{key}_DESCRIPTION"));
                __instance._toolTipSubName.text = string.Format(
                    Localyssation.GetString("FORMAT_ITEM_RARITY", __instance._toolTipSubName.text, __instance._toolTipSubName.fontSize),
                    Localyssation.GetString(KeyUtil.GetForAsset(__instance._scriptItem._itemRarity), __instance._scriptItem._itemRarity.ToString(), __instance._toolTipSubName.fontSize));
                
                if (__instance._scriptItem._itemType != ItemType.GEAR)
                    __instance._itemToolTipType.text = Localyssation.GetString(KeyUtil.GetForAsset(__instance._scriptItem._itemType));
            }
        }

        [HarmonyPatch(typeof(ItemToolTip), nameof(ItemToolTip.Apply_ItemStats))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemToolTip_Apply_ItemStats_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // TODO: 失效了
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "Mystery Item", "ITEM_TOOLTIP_GAMBLE_ITEM_NAME" },
                { "[Unknown]", "ITEM_TOOLTIP_GAMBLE_ITEM_RARITY" },
                { "You can't really see what __instance is until you buy it.", "ITEM_TOOLTIP_GAMBLE_ITEM_DESCRIPTION" },
                { "Recovers {0} Health.", "ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_HEALTH_APPLY" },
                { "Recovers {0} Mana.", "ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_MANA_APPLY" },
                { "Recovers {0} Stamina.", "ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_STAMINA_APPLY" },
                { "Gain {0} Experience on use.", "ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_EXP_GAIN" },
                { "Consumable", "ITEM_TOOLTIP_TYPE_CONSUMABLE" },
                { "Trade Item", "ITEM_TOOLTIP_TYPE_TRADE" },
            });
        }

        [HarmonyPatch(typeof(ItemObjectVisual), nameof(ItemObjectVisual.Apply_ItemObjectVisual))]
        [HarmonyPostfix]
        public static void ItemObjectVisual_Apply_ItemObjectVisual(ItemObjectVisual __instance)
        {
            if (__instance._itemObject._currencyDropAmount > 0)
                Apply_CurrencyVisual();
            else
                Apply_ItemVisual();

            void Apply_CurrencyVisual()
            {
                int currencyDropAmount = __instance._itemObject._currencyDropAmount;
                __instance._itemNametagTextMesh.text = string.Format("{0:n0} ", currencyDropAmount) + 
                    Localyssation.GetString(currencyDropAmount > 1 ? RTLore.CROWN_PLURAL : RTLore.CROWN);
            }

            void Apply_ItemVisual()
            {
                string str = "";
                if (__instance._itemObject._foundItem._itemType == ItemType.GEAR)
                {
                    __instance._itemVisualErrorSpriteRend.gameObject.SetActive(!((ScriptableEquipment)__instance._itemObject._foundItem).CanEquipItem(Player._mainPlayer._pStats, false));
                    if (__instance._itemObject._local_itemData._modifierID > 0)
                    {
                        // modifier
                        str = Localyssation.GetString(
                            KeyUtil.GetForAsset(
                                GameManager._current.Locate_StatModifier(__instance._itemObject._local_itemData._modifierID)
                                )
                            ) + " ";
                        
                    }
                }
                __instance._itemNametagTextMesh.text = str + Localyssation.GetString(KeyUtil.GetForAsset(__instance._itemObject._foundItem));
            }
        }
    }
}
