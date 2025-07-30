using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Localyssation.Patches.ReplaceText
{
    internal static class MessageCallbacks
    {
        public static readonly MethodInfo Start_QuickSentence
            = AccessTools.Method(
                typeof(DialogManager),
                nameof(DialogManager.Start_QuickSentence),
                new[] { typeof(string) }
                );
        public static readonly MethodInfo Init_GameLogicMessage
            = AccessTools.Method(
                typeof(ChatBehaviour),
                nameof(ChatBehaviour.Init_GameLogicMessage),
                new[] { typeof(string) }
                );
        public static readonly MethodInfo New_ChatMessage
            = AccessTools.Method(
                typeof(ChatBehaviour),
                nameof(ChatBehaviour.New_ChatMessage),
                new[] { typeof(string) }
                );

    }
}
