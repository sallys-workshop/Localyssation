using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Localyssation.Util;
using Mirror;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(NetTrigger), nameof(NetTrigger.Init_SendTriggerMessage))]
        [HarmonyPrefix]
        private static bool NetTrigger__Init_SendTriggerMessage__Postfix(NetTrigger __instance)
        {
            var baseKey = KeyUtil.GetForAsset(__instance);
            if (!NetworkServer.active)
                return false;
            string _msg = baseKey.SingleMessage.Localize();
            if (baseKey.MessageArrayLength != 0)
                _msg = baseKey.MessageArray(UnityEngine.Random.Range(0, baseKey.MessageArrayLength)).Localize();
            if (string.IsNullOrEmpty(_msg))
                return false;
            foreach (ChatBehaviour chatBehaviour in UnityEngine.Object.FindObjectsOfType<ChatBehaviour>())
            {
                if (chatBehaviour != null && chatBehaviour.gameObject.scene == __instance.gameObject.scene)
                {
                    if (!__instance._triggerMessage._sentMsg)
                        chatBehaviour.Target_RecieveTriggerMessage(_msg);
                    else
                        break;
                }
            }
            __instance._triggerMessage._sentMsg = true;
            return false;
        }
    }
}
