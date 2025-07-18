using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

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
        internal static IEnumerable<CodeInstruction> SimpleStringReplaceTranspiler(
            IEnumerable<CodeInstruction> instructions, 
            IDictionary<string, string> stringReplacements, 
            bool allowRepeat=false, bool supressNotfoundWarnings = false
        )
        {
            var replacedStrings = new List<string>();
            
            var result = new CodeMatcher(instructions).MatchForward(false,
                new CodeMatch(
                    (instr) => 
                        instr.opcode == OpCodes.Ldstr 
                        && stringReplacements.ContainsKey((string)instr.operand) 
                        && (allowRepeat || !replacedStrings.Contains((string)instr.operand))
                ))
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
            var notReplaced = stringReplacements.Keys.Cast<string>().Except(replacedStrings).ToList();
            if (notReplaced.Count > 0 && !supressNotfoundWarnings)
                Localyssation.logger.LogWarning("Some strings are not found during transpiler replacing:\n\t" + string.Join("\n\t", notReplaced));
            return result;
        }

        internal static IEnumerable<CodeInstruction> SimpleStringReplaceTranspiler(
            IEnumerable<CodeInstruction> instructions, 
            IEnumerable<string> keyReplacement, 
            bool allowRepeat = false, bool supressNotfoundWarnings = false
        )
        {
            Dictionary<string, string> stringReplacements = new Dictionary<string, string>();
            
            foreach (var key in keyReplacement)
            {
                stringReplacements.Add(I18nKeys.getDefaulted(key), key);
            }
            return SimpleStringReplaceTranspiler(instructions, stringReplacements, allowRepeat, supressNotfoundWarnings);
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
        public static void RemapChildTextsByPath(Transform parentTransform, IDictionary<string, string> textRemaps, Action<Transform, string> onRemap = null, bool supressNotfoundWarnings = false, bool rawText = false)
        {
            foreach (var textRemap in textRemaps)
            {
                //Localyssation.logger.LogDebug(textRemap);
                var foundTransform = parentTransform.Find(textRemap.Key);
                if (foundTransform)
                {
                    var text = foundTransform.GetComponent<Text>();
                    if (text)
                    {
                        if (!rawText)
                            LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc(textRemap.Value, text.text));
                        else
                            text.text = textRemap.Value;
                        if (onRemap != null) onRemap(foundTransform, textRemap.Value);
                    }
                    else
                    {
                        if (!supressNotfoundWarnings)
                            Localyssation.logger.LogWarning($"[RemapChildTextsByPath] Found path `{textRemap.Key}` but no Text component is found.");
                    }
                }
                else
                {
                    if (!supressNotfoundWarnings)
                        Localyssation.logger.LogWarning($"[RemapChildTextsByPath] Cannot find path `{textRemap.Key}` in `{GetPath(parentTransform)}`.");
                }
            }
        }

        public static string GetPath(Transform transform)
        {
            string path = transform.name;
            Transform current = transform;

            // 从当前节点向上遍历到根节点
            while (current.parent != null)
            {
                current = current.parent;
                path = current.name + "/" + path; // 从根向子节点拼接
            }

            return "/" + path; // 添加根路径斜杠
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

        private static readonly ImmutableList<Type> PATCH_CLASSES = ImmutableList.Create(
            typeof(RTReplacer)
        );

        public static void PatchAll(Harmony harmony)
        {
            foreach (var item in PATCH_CLASSES)
            {
                harmony.PatchAll(item);
            }
        }

        public static string Capitalize(string text)
        {
            return text[0].ToString().ToUpper() + text.Substring(1);
        }

        public static RTTransplierCodeInstructionsWrapper Wrap(IEnumerable<CodeInstruction> instructions)
        {
            return new RTTransplierCodeInstructionsWrapper(instructions);
        }

    }

    /// <summary>
    /// For chained usage
    /// </summary>
    internal class RTTransplierCodeInstructionsWrapper
    {
        private IEnumerable<CodeInstruction> __instructions;
        public RTTransplierCodeInstructionsWrapper(IEnumerable<CodeInstruction> instructions)
        {
            __instructions = instructions;
        }

        public RTTransplierCodeInstructionsWrapper ReplaceStrings(
            IDictionary<string, string> stringReplacements,
            bool allowRepeat = false, bool supressNotfoundWarnings = false
        )
        {
            this.__instructions = RTUtil.SimpleStringReplaceTranspiler(__instructions, stringReplacements, allowRepeat, supressNotfoundWarnings);
            return this;
        }

        public RTTransplierCodeInstructionsWrapper ReplaceStrings(
            IEnumerable<string> stringReplacements,
            bool allowRepeat = false, bool supressNotfoundWarnings = false
        )
        {
            __instructions = RTUtil.SimpleStringReplaceTranspiler(__instructions, stringReplacements, allowRepeat, supressNotfoundWarnings);
            return this;
        }

        public IEnumerable<CodeInstruction> Unwrap()
        { 
            return __instructions;
        }

    }
}
