using HarmonyLib;
using Localyssation.LangAdjutable;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRItemObjectVisual
    {
        [HarmonyPatch(typeof(ItemObjectVisual), nameof(ItemObjectVisual.OnEnable))]
        [HarmonyPostfix]
        public static void ItemObjectVisual__OnEnable__Postfix(ItemObjectVisual __instance)
        {
            LangAdjustables.RegisterTextMeshPro(__instance._itemNametagTextMesh);
        }
    }
}
