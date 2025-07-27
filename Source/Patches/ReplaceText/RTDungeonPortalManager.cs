using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(DungeonPortalManager), nameof(DungeonPortalManager.Update))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DungeonPortalManager__Update__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new string[]
            {
                I18nKeys.Lore.DUNGEON_PORTAL_ENTER_LEVELED_FORMAT
            }, allowRepeat: true);
        }

        [HarmonyPatch(typeof(DungeonPortalManager), nameof(DungeonPortalManager.Update))]
        [HarmonyPostfix]
        public static void DungeonPortalManager__Update__Postfix(DungeonPortalManager __instance)
        {
            __instance._dungeonNameHeaderText.text = "- " + Localyssation.GetString(KeyUtil.GetForMapName(__instance._scenePortal._portalCaptionTitle)) + " -";
        }
    }
}
