using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(WorldPortalManager), nameof(WorldPortalManager.Update))]
        [HarmonyPostfix]
        public static void WorldPortalManager__Update__Postfix(WorldPortalManager __instance)
        {
            if ((bool)__instance._selectedWorldPortalEntry)
            {
                __instance._mapEntryHeaderText.text = $" - {Localyssation.GetString(KeyUtil.GetForMapName(__instance._selectedWorldPortalEntry._scriptMapData._mapCaptionTitle))} -";
            }
            else
            {
                __instance._mapEntryHeaderText.text = Localyssation.GetString(I18nKeys.Lore.WORLDPORTAL_SELECT_WAYPOINT);
            }
        }

        [HarmonyPatch(typeof(WorldPortalManager), nameof(WorldPortalManager.Awake))]
        [HarmonyPostfix]
        public static void WorldPortalManager__Awake__Postfix(WorldPortalManager __instance)
        {
            RTUtil.RemapAllTextUnderObject(__instance.gameObject, new Dictionary<string, string>()
            {
                { "_text_worldPortal_header", I18nKeys.Lore.WORLDPORTAL_TITLE},
                { "_button_teleportWorldPortal", I18nKeys.Lore.WORLDPORTAL_TELEPORT }
            });
        }
    }
}
