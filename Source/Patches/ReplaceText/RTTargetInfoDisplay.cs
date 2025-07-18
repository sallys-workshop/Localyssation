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
        [HarmonyPatch(typeof(TargetInfoDisplayManager), nameof(TargetInfoDisplayManager.Handle_TargetInfoDisplay))]
        [HarmonyPostfix]
        public static void TargetInfoDisplayManager_Handle_TargetInfoDisplay_Postfix(TargetInfoDisplayManager __instance)
        {
            if (!Player._mainPlayer)
            {
                return;
            }
            if (!__instance._setStatusEntityTarget
                || Player._mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE)
            {
                return;
            }
            if ((bool)__instance._setStatusEntityTarget._isCreep)
            {
                Creep creep = __instance._setStatusEntityTarget._isCreep;
                /// __instance._targetNameField.text = creep._creepDisplayName;
                /// _creepDisplayName = modifier + baseName
                /// <see cref="Creep.Server_HandleBufferParams">

                if ((bool)creep._scriptStatModifier)
                {
                    //__instance._targetNameField.text 
                    //    = creep._scriptStatModifier._modifierTag + " " 
                    //    + creep._scriptCreep._creepName;
                    __instance._targetNameField.text
                        = Localyssation.GetString(KeyUtil.GetForAsset(creep._scriptStatModifier) + "_TAG")
                        + " "
                        + Localyssation.GetString(KeyUtil.GetForAsset(creep._scriptCreep) + "_NAME");
                }
                else
                {
                    __instance._targetNameField.text = Localyssation.GetString(KeyUtil.GetForAsset(creep._scriptCreep) + "_NAME");
                }
            }
        } 
    }
}
