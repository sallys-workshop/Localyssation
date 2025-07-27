using HarmonyLib;
using Localyssation.LanguageModule;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRPlayerNickname
    {


        [HarmonyPatch(typeof(Player), nameof(Player.Handle_ClientParameters))]
        [HarmonyPostfix]
        public static void Player_Handle_ClientParameter_Postfix(Player __instance)
        {
            if (__instance._nicknameTextMesh.enabled)
                FRUtil.ReplaceTmpFont(__instance._nicknameTextMesh, LanguageManager.CurrentLanguage.info.chatFont);
            if (__instance._globalNicknameTextMesh.enabled)
                FRUtil.ReplaceTmpFont(__instance._globalNicknameTextMesh, LanguageManager.CurrentLanguage.info.chatFont);
        }
    }
}
