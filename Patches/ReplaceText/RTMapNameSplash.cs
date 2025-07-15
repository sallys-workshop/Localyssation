using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Collections;

namespace Localyssation.Patches.ReplaceText
{

    ///_GameUI_InGame/Canvas_InGameUI/dolly_mapNameTitle/
    ///  _text_mapNameTitle, _text_mapZoneTypeTitle
    /// <see cref="InGameUI._mapNameText"/>
    /// <see cref="InGameUI._mapZoneTypeText"/>
    /// <param name="_reigonTag"/> ="" for map change, = map region in colliders in scene

    [HarmonyPatch(typeof(InGameUI))]
    [HarmonyPatch(nameof(InGameUI.MapTitleDisplay))]
    public class InGameUI_MapTitleDisplay_Patch
    {
        public static bool Prefix(InGameUI __instance, string _reigonTag, ref IEnumerator __result)
        {
            _reigonTag = Localyssation.GetString(KeyUtil.GetForMapRegionTag(_reigonTag));
            __result = InGameUI_MapTitleDisplay_CustomIEnumerator(__instance, _reigonTag);
            return false;
        }

        private static IEnumerator InGameUI_MapTitleDisplay_CustomIEnumerator(InGameUI __instance, string _reigonTag)
        {
            __instance._reigonTitle = _reigonTag;
            do
            {
                yield return null;
            }
            while (Player._mainPlayer._currentGameCondition == GameCondition.LOADING_GAME || Player._mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE || Player._mainPlayer._bufferingStatus);
            if (!string.IsNullOrWhiteSpace(_reigonTag))
            {
                __instance._mapNameText.text = _reigonTag ?? "";
            }
            else
            {
                __instance._mapNameText.text = Localyssation.GetString(KeyUtil.GetForMapName(Player._mainPlayer.Network_playerMapInstance._mapName)) ?? "";
            }

            __instance._mapZoneTypeText.text = string.Format(
                Localyssation.GetString(I18nKeys.Lore.FORMAT_MAP_ZONE),
                Localyssation.GetString(KeyUtil.GetForAsset(Player._mainPlayer._playerZoneType))
            );
            yield return new WaitForSeconds(1.5f);
            do
            {
                if (TabMenu._current._isOpen)
                {
                    yield break;
                }

                CanvasGroup mapInstanceTitleGroup = __instance._mapInstanceTitleGroup;
                mapInstanceTitleGroup.alpha += Time.deltaTime * 0.85f;
                yield return null;
            }
            while (__instance._mapInstanceTitleGroup.alpha < 1f);
            yield return new WaitForSeconds(2.35f);
            while (!TabMenu._current._isOpen)
            {
                CanvasGroup mapInstanceTitleGroup2 = __instance._mapInstanceTitleGroup;
                mapInstanceTitleGroup2.alpha -= Time.deltaTime * 0.85f;
                yield return null;
                if (!(__instance._mapInstanceTitleGroup.alpha > 0f))
                {
                    break;
                }
            }
        }

    }

}
