using HarmonyLib;
using Localyssation.LangAdjutable;
using UnityEngine;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRItemObjectVisual
    {
        [HarmonyPatch(typeof(ItemObjectVisual), nameof(ItemObjectVisual.OnEnable))]
        [HarmonyPostfix]
        public static void ItemObjectVisual__OnEnable__Postfix(ItemObjectVisual __instance)
        {
            if (__instance != null && __instance._itemNametagTextMesh)
            {
                FRUtil.replaceTmpFont(__instance._itemNametagTextMesh, Localyssation.currentLanguage.info.chatFont);
            }
        }

    }
}
