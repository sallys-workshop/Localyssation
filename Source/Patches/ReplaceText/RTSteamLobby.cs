using HarmonyLib;
using Localyssation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                { "Canvas_SteamLobbyHost/_dolly_hostWindow/_dolly_serverSettings/_backdrop_header/_text_lobbyHostHeader", LOBBY_HOST_HEADER },
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
                { "Canvas_SteamLobbyHost/_dolly_hostWindow/_dolly_serverSettings/_backdrop_hostLobbySettings/_dolly_hostLobbySettings/_input_motd/Placeholder", PLACEHOLDER_MOTD }
            });
        }
    }
}
