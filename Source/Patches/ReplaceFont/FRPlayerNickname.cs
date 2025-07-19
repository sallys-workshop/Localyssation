using System;
using HarmonyLib;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRPlayerNickname
    {
        

        [HarmonyPatch(typeof(Player), nameof(Player.Handle_ClientParameters))]
        [HarmonyPostfix]
        public static void Player_Handle_ClientParameter_Postfix(Player __instance)
        {
            if (__instance._nicknameTextMesh.enabled)
                FRUtil.replaceTmpFont(__instance._nicknameTextMesh, Localyssation.currentLanguage.info.chatFont);
            if (__instance._globalNicknameTextMesh.enabled)
                FRUtil.replaceTmpFont(__instance._globalNicknameTextMesh, Localyssation.currentLanguage.info.chatFont);
        }
    }
}
