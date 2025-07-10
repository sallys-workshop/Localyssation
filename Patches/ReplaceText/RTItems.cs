using HarmonyLib;
using System.Collections.Generic;

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
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "Mystery Item", "ITEM_TOOLTIP_GAMBLE_ITEM_NAME" },
                { "[Unknown]", "ITEM_TOOLTIP_GAMBLE_ITEM_RARITY" },
                { "You can't really see what this is until you buy it.", "ITEM_TOOLTIP_GAMBLE_ITEM_DESCRIPTION" },
                { "Recovers {0} Health.", "ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_HEALTH_APPLY" },
                { "Recovers {0} Mana.", "ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_MANA_APPLY" },
                { "Recovers {0} Stamina.", "ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_STAMINA_APPLY" },
                { "Gain {0} Experience on use.", "ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_EXP_GAIN" },
                { "Consumable", "ITEM_TOOLTIP_TYPE_CONSUMABLE" },
                { "Trade Item", "ITEM_TOOLTIP_TYPE_TRADE" },
            });
        }

    }
}
