using HarmonyLib;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRItemObjectVisual
    {
        [HarmonyPatch(typeof(ItemObjectVisual), nameof(ItemObjectVisual.Apply_ItemObjectVisual))]
        [HarmonyPostfix]
        public static void modifyItemObjectVisualFont(ItemObjectVisual __instance)
        {
            if (__instance != null && __instance._itemNametagTextMesh.enabled)
            {
                FRUtil.replaceTmpFont(__instance._itemNametagTextMesh, Localyssation.currentLanguage.info.fontReplacementLibrationSans);
            }
            
        }
    }
}
