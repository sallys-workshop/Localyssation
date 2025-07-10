using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;
namespace Localyssation.Patches.ReplaceText
{
    internal static class RTUtil
    {
        // general utils

        /// <summary>
        /// Creates a CodeMatcher that remaps strings found in the game with localised versions found in the current language.
        /// </summary>
        /// <param name="instructions">IEnumerable provided by the Harmony transpiler.</param>
        /// <param name="stringReplacements">Key-value pairs of in-game strings to replace and language keys to replace with.</param>
        /// <returns>The generated CodeMatcher.</returns>
        internal static IEnumerable<CodeInstruction> SimpleStringReplaceTranspiler(IEnumerable<CodeInstruction> instructions, Dictionary<string, string> stringReplacements)
        {
            var replacedStrings = new List<string>();
            return new CodeMatcher(instructions).MatchForward(false,
                new CodeMatch((instr) => instr.opcode == OpCodes.Ldstr && stringReplacements.ContainsKey((string)instr.operand) && !replacedStrings.Contains((string)instr.operand)))
                .Repeat(matcher =>
                {
                    var key = (string)matcher.Instruction.operand;
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(Transpilers.EmitDelegate<Func<string, string>>((origString) =>
                    {
                        return Localyssation.GetString(stringReplacements[key], key);
                    }));
                    replacedStrings.Add(key);
                }).InstructionEnumeration();
        }

        /// <summary>
        /// Gets the operand from the current CodeMatcher instruction. If none found, tries to parse the last symbol of the instruction's OpCode name as an integer.
        /// </summary>
        /// <param name="matcher">The CodeMatcher to get the instruction operand from.</param>
        /// <returns>The int operand.</returns>
        internal static int GetIntOperand(CodeMatcher matcher)
        {
            var result = matcher.Operand;
            if (result == null)
                result = int.Parse(matcher.Opcode.Name.Substring(matcher.Opcode.Name.Length - 1));
            return (int)result;
        }

        /// <summary>
        /// Remaps all UnityEngine.UI.Text instances found in this object, and under all of its children, with localised variants if the GameObject names match remap keys.
        /// </summary>
        /// <param name="gameObject">The GameObject to find Text instances in.</param>
        /// <param name="textRemaps">Key-value pairs of GameObject names to find and language keys to replace their text with.</param>
        /// <param name="onRemap">A method called on a successful remap.</param>
        public static void RemapAllTextUnderObject(GameObject gameObject, Dictionary<string, string> textRemaps, Action<Transform, string> onRemap = null)
        {
            bool TryRemapSingle(Transform lookupNameTransform, Text text)
            {
                if (textRemaps.TryGetValue(lookupNameTransform.name, out var key))
                {
                    LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc(key, text.text));
                    if (onRemap != null) onRemap(lookupNameTransform, key);
                    return true;
                }
                return false;
            }

            foreach (var text in gameObject.GetComponentsInChildren<Text>())
            {
                if (!TryRemapSingle(text.transform, text))
                {
                    var textParent = text.transform.parent;
                    if (textParent) TryRemapSingle(textParent, text);
                }
            }
        }

        /// <summary>
        /// Remaps all UnityEngine.UI.Text instances found under the parent transform with localised variants if the child GameObject paths match remap keys.
        /// </summary>
        /// <param name="parentTransform">The Transform to find Text instances under.</param>
        /// <param name="textRemaps">Key-value pairs of GameObject paths to find and language keys to replace their text with.</param>
        /// <param name="onRemap">A method called on a successful remap.</param>
        public static void RemapChildTextsByPath(Transform parentTransform, Dictionary<string, string> textRemaps, Action<Transform, string> onRemap = null)
        {
            foreach (var textRemap in textRemaps)
            {
                var foundTransform = parentTransform.Find(textRemap.Key);
                if (foundTransform)
                {
                    var text = foundTransform.GetComponent<Text>();
                    if (text)
                    {
                        LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc(textRemap.Value, text.text));
                        if (onRemap != null) onRemap(foundTransform, textRemap.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Remaps all UnityEngine.UI.InputField instances' placeholder texts found in this object, and under all of its children, with localised variants if the in-game strings match remap keys.
        /// </summary>
        /// <param name="gameObject">The GameObject to find InputField instances in.</param>
        /// <param name="textRemaps">Key-value pairs of in-game strings to replace and language keys to replace with.</param>
        /// <param name="onRemap">A method called on a successful remap.</param>
        public static void RemapAllInputPlaceholderTextUnderObject(GameObject gameObject, Dictionary<string, string> textRemaps, Action<Transform, string> onRemap = null)
        {
            foreach (var inputField in gameObject.GetComponentsInChildren<InputField>())
            {
                if (inputField.placeholder)
                {
                    var text = inputField.placeholder.GetComponent<Text>();
                    if (text && textRemaps.TryGetValue(inputField.name, out var key))
                    {
                        LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc(key, text.text));
                        if (onRemap != null) onRemap(inputField.transform, key);
                    }
                }
            }
        }

        private static List<string> fallbackTextEditTags = new List<string>()
        {
            "scalefallback"
        };

        public static List<string> getFallbackTextEditTags()
        {
            return fallbackTextEditTags;
        }


    }

    internal static class RTLore
    {
        public const string CROWN = "CROWN";
        public const string CROWN_PLURAL = "CROWN_PLURAL";
    }
}
