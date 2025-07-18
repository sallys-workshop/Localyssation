using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine.UI;

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
                RTUtil.RemapChildTextsByPath(__instance._primaryLoadoutIcon.transform, new Dictionary<string, string> { { "Text (Legacy)" , "I"} }, rawText:true);
            if (__instance._altLoadoutIcon)
                RTUtil.RemapChildTextsByPath(__instance._altLoadoutIcon.transform, new Dictionary<string, string> { { "Text (Legacy)", "II" } }, rawText:true);
        }

        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Awake))]
        [HarmonyPostfix]
        public static void ItemMenuCell__Awake__Postfix(ItemMenuCell __instance)
        {
            RTUtil.RemapChildTextsByPath(__instance.transform, WeaponSlotReplacer(
                "_equipmentTab/_dolly_equipCells/_dolly_lowerColumn/GameObject/_equipcell_{0}eapon/_quickWepSlot_numIco_{1}/Text (Legacy)",
                    "w",
                    "altW",
                    "01",
                    "02"
            ), supressNotfoundWarnings: false, rawText: true);
        }
    }
}
