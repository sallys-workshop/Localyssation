using HarmonyLib;
using System.Collections.Generic;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.UserCode_Cmd_DropItem__ItemData__Int32))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlayerDropItem_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new[] {
                I18nKeys.Feedback.DROP_ITEM_FORMAT
            });
        }
    }
}
