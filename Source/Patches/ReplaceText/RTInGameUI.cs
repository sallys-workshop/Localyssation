using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        private static IDictionary<string, string> WeaponSlotReplacer(string basePathFormat, string pParent1, string pParent2, string pChild1, string pChild2)
        {
            return new Dictionary<string, string> {
                { string.Format(basePathFormat, pParent1, pChild1), "I"},
                { string.Format(basePathFormat, pParent1, pChild2), "II" },
                { string.Format(basePathFormat, pParent2, pChild1), "I"},
                { string.Format(basePathFormat, pParent2, pChild2), "II" },
            };
        }

        [HarmonyPatch(typeof(InGameUI), nameof(InGameUI.Awake))]
        [HarmonyPostfix]
        public static void InGameUI__Awake__Postfix(InGameUI __instance)
        {
            //QuickSwapWeaponGUI

            RTUtil.RemapChildTextsByPath(__instance.transform,
                WeaponSlotReplacer("Canvas_InGameUI/dolly_bottomBar/_cell_weaponSwapper/{0}/{1}/Text (Legacy)",
                    "_altWeaponQuickSlot",
                    "_weaponQuickSlot",
                    "_quickWepSlot_numIco_01",
                    "_quickWepSlot_numIco"
                )
            , supressNotfoundWarnings: true
            , rawText: true
            );

        }
        [HarmonyPatch(typeof(SkillListDataEntry), nameof(SkillListDataEntry.Start))]
        [HarmonyPostfix]
        public static void SkillListDataEntry__Start__Postfix(SkillListDataEntry __instance)
        {
            if (__instance._primaryLoadoutIcon)
                RTUtil.RemapChildTextsByPath(__instance._primaryLoadoutIcon.transform, new Dictionary<string, string> { { "Text (Legacy)", "I" } }, rawText: true);
            if (__instance._altLoadoutIcon)
                RTUtil.RemapChildTextsByPath(__instance._altLoadoutIcon.transform, new Dictionary<string, string> { { "Text (Legacy)", "II" } }, rawText: true);
        }

        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Start))]
        [HarmonyPostfix]
        public static void ItemMenuCell__Start__Postfix(ItemMenuCell __instance)
        {
            RTUtil.RemapChildTextsByPath(__instance.transform, WeaponSlotReplacer(
                "_equipmentTab/_dolly_equipCells/_dolly_lowerColumn/GameObject/_equipcell_{0}eapon/_quickWepSlot_numIco_{1}/Text (Legacy)",
                    "w",
                    "altW",
                    "01",
                    "02"
            ), supressNotfoundWarnings: true, rawText: true);
        }

        [HarmonyPatch(typeof(ActionBarManager), nameof(ActionBarManager.Handle_CastBar))]
        [HarmonyPostfix]
        public static void ActionBarManager__Handle_CastBar__Postfix(ActionBarManager __instance)
        {
            if (__instance._player._currentPlayerAction == PlayerAction.CASTING && __instance._pCast && __instance._pCast._currentCastSkill)
                __instance._castBarTag.text = Localyssation.GetString(KeyUtil.GetForAsset(__instance._pCast._currentCastSkill) + "_NAME");
        }
    }
}
