using HarmonyLib;
using Localyssation.LangAdjutable;
using Localyssation.Util;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Localyssation.I18nKeys.SteamLobby;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.Awake))]
        [HarmonyPostfix]
        static void SteamManager__Awake__Postfix(SteamManager __instance)
        {
            
            string LobbyTag(string name) => "Canvas_SteamLobbyHost/_dolly_hostWindow/_dolly_serverSettings/_backdrop_hostLobbySettings/_dolly_hostLobbyTags/" + name;
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, TranslationKey> {
                //{ "Canvas_SteamLobbyHost/_dolly_hostWindow/_dolly_serverSettings/_backdrop_header/_text_lobbyHostHeader", LOBBY_HOST_HEADER },
                { LobbyTag("_tag_lobbyName"), TAG_LOBBY_NAME },
                { LobbyTag("_tag_password"), TAG_LOBBY_PASSWORD },
                { LobbyTag("_tag_motd"), TAG_MOTD },
                { LobbyTag("_tag_lobbyType"), TAG_LOBBY_TYPE},
                { LobbyTag("_tag_maxPlayers"), TAG_MAX_PLAYERS },
                { LobbyTag("_tag_streamMode"), TAG_STREAM_MODE },
                { LobbyTag("_tag_lobbyFocusType"), TAG_LOBBY_REALM },
                { "Canvas_SteamLobbyHost/_dolly_hostWindow/_dolly_serverSettings/_backdrop_hostLobbyButtons/_button_cancelHostLobby/Text (Legacy)", BUTTON_RETURN },
                { "Canvas_SteamLobbyHost/_dolly_hostWindow/_dolly_serverSettings/_backdrop_hostLobbyButtons/_button_hostLobby/Text (Legacy)", BUTTON_HOST_LOBBY },
                { "Canvas_SteamLobbyHost/_dolly_hostWindow/_dolly_serverSettings/_backdrop_hostLobbySettings/_dolly_hostLobbySettings/_input_lobbyName/Placeholder", PLACEHOLDER_LOBBY_NAME },
                { "Canvas_SteamLobbyHost/_dolly_hostWindow/_dolly_serverSettings/_backdrop_hostLobbySettings/_dolly_hostLobbySettings/_input_lobbyPassword/Placeholder", PLACEHOLDER_LOBBY_PASSWORD},
                { "Canvas_SteamLobbyHost/_dolly_hostWindow/_dolly_serverSettings/_backdrop_hostLobbySettings/_dolly_hostLobbySettings/_input_motd/Placeholder", PLACEHOLDER_MOTD },

                { "Canvas_SteamLobbyFinder/_dolly_lobbyFinderWindow/_dolly_finderOptions/_cancelLobbyFinderButton", BUTTON_RETURN},
                { "Canvas_SteamLobbyFinder/_dolly_lobbyFinderWindow/_hiddenLobbyPanel/_dolly_hiddenLobbyPanel/_clearHiddenLobbiesButton/Text (Legacy)", CLEAR_HIDDEN_LOBBY_BUTTON }

            });
        }

        [HarmonyPatch(typeof(LobbyListManager), nameof(LobbyListManager.Awake))]
        [HarmonyPostfix]
        static void LobbyListManager__Awake__Postfix(LobbyListManager __instance)
        {
            LangAdjustables.RegisterDropdown(__instance.dropDown_lobbyListFilter, LOBBY_LIST_FILTER_BASE);
            LangAdjustables.RegisterDropdown(__instance._lobbyTypeDropdown, LOBBY_TYPE_BASE);
            RTUtil.RemapAllInputPlaceholderTextUnderObject(__instance._input_lobbyNameSearch.gameObject,
                new Dictionary<string, string> { { "Placeholder", PLACEHOLDER_LOBBY_NAME_SEARCH } });
            RTUtil.RemapAllTextUnderObject(__instance._cancelButton.gameObject, new Dictionary<string, string>() {
                { "Text (Legacy)", BUTTON_RETURN }
            });
        }

        //[HarmonyPatch(typeof(LobbyListManager))]
        [HarmonyPatch(typeof(LobbyListManager), nameof(LobbyListManager.Init_ClearHiddenLobbyCache))]
        [HarmonyPatch(typeof(LobbyListManager), nameof(LobbyListManager.Update))]
        [HarmonyPatch(typeof(LobbyListManager), nameof(LobbyListManager.Handle_HostingParameters))]
        [HarmonyPatch(typeof(LobbyListManager), nameof(LobbyListManager.Init_RefreshLobbyList))]
        [HarmonyPatch(typeof(LobbyListManager), nameof(LobbyListManager.RefreshButtonBuffer))]
        [HarmonyPatch(typeof(LobbyListManager), nameof(LobbyListManager.Iterate_SteamLobbies))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> LobbyListManager__ALL__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new[]
                {
                    // LobbyListManager.Init_ClearHiddenLobbyCache
                    CLEARED_HIDDEN_LOBBY_CACHE,

                    // LobbyListManager.Update
                    HIDDEN_LOBBY_COUNT_FORMAT,

                    // LobbyListManager.Handle_HostingParameters
                    LOBBY_HOST_HEADER,
                    LOBBY_HOST_HEADER_STEAM_UNAVAILABLE,

                    LOBBY_TYPE_DESCRIPTION_FRIENDS,
                    LOBBY_TYPE_DESCRIPTION_PUBLIC,
                    LOBBY_TYPE_DESCRIPTION_PRIVATE,

                    // Init_RefreshLobbyList
                    STEAM_NOT_INITIALIZED,
                    SEARCHING_FOR_LOBBIES,

                    // RefreshButtonBuffer
                    NO_LOBBIES_FOUND,

                    // Iterate_SteamLobbies
                    LOBBY_FOUNDED_COUNT_FORMAT_1,
                    LOBBY_FOUNDED_COUNT_FORMAT_PLURAL
                }, allowRepeat:true, supressNotfoundWarnings:true)
                .Unwrap();
        }

        [HarmonyPatch(typeof(LobbyDataEntry), nameof(LobbyDataEntry.SetLobbyData))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> LobbyDataEntry__SetLobbyData__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new[]
                {
                    UNTITLED_LOBBY,
                    LOBBY_PLAYER_COUNT,
                    LOBBY_PING_FORMAT
                })
                .Unwrap();
        }

        [HarmonyPatch(typeof(LobbyDataEntry), nameof(LobbyDataEntry.SetLobbyData))]
        [HarmonyPostfix]
        static void LobbyDataEntry__SetLobbyData__Postfix(LobbyDataEntry __instance)
        {
            __instance._joinLobbyButtonText.text = JOIN_LOBBY.Localize();
            RTUtil.RemapChildTextsByPath(__instance._moddedLobbyTag.transform, new Dictionary<string, string>() { { "_text_moddedLobby", MODDED_LOBBY } });
        }
    }

    [HarmonyPatch]
    class LobbyListManager__Iterate_SteamLobbies__Iterate_LobbyEntry__Transpiler
    {
        private static readonly TargetInnerMethod TARGET = new TargetInnerMethod {
            Type = typeof(LobbyListManager),
            ParentMethodName = nameof(LobbyListManager.Iterate_SteamLobbies),
            InnerMethodName = "Iterate_LobbyEntry"
        };

        static MethodBase TargetMethod() => TranspilerHelper.GenerateTargetMethod(TARGET);

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new[]
                {
                    LOBBY_FULL,
                    LOBBY_PASSWORD_LOBBY,
                    LOBBY_JOIN_FRIEND,
                    LOBBY_DIFFERENT_VERSION,
                    LOBBY_INVALID
                })
                .Unwrap();
        }
    }
}
