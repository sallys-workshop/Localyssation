using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        // 
        //_GameUI_InGame/Canvas_InGameUI/dolly_mapNameTitle/
        //  _text_mapNameTitle, _text_mapZoneTypeTitle
        /// <see cref="InGameUI._mapNameText"/>
        /// <see cref="InGameUI._mapZoneTypeText"/>
        /// <param name="_reigonTag"/> ="" for map change, = map region in colliders in scene
        [HarmonyPatch(typeof(InGameUI), nameof(InGameUI.MapTitleDisplay))]
        [HarmonyPostfix]
        public static void InGameUI_MapTitleDisplay_Postfix(InGameUI __instance, string _reigonTag)
        {
            if (!string.IsNullOrWhiteSpace(_reigonTag))
            {
                __instance._mapNameText.text = Localyssation.GetString(KeyUtil.GetForMapRegionTag(_reigonTag)) ?? "";
            }
            else
            {
                __instance._mapNameText.text = Player._mainPlayer.Network_playerMapInstance._mapName ?? "";
            }
            __instance._mapZoneTypeText.text = string.Format(
                Localyssation.GetString(I18nKeys.Lore.FORMAT_MAP_ZONE),
                Localyssation.GetString(KeyUtil.GetForAsset(Player._mainPlayer._playerZoneType))
            );
        }

    }
}
