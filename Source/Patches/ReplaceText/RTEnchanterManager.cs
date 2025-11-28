using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(EnchanterManager), nameof(EnchanterManager.Awake))]
        [HarmonyPostfix]
        public static void EnchanterManager_Awake_Postfix(EnchanterManager __instance)
        {
            string controllerButton(string name)
            {
                return $"Canvas_DialogSystem/_dolly_enchanterBox/_backdrop_enchantItem/_button_{name}/_button_{name}Text";
            }
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>()
            {
                { "Canvas_DialogSystem/_dolly_enchanterBox/_backdrop_header/_text_header", I18nKeys.Enchanter.HEADER },
                { controllerButton("clearEnchant"), I18nKeys.Enchanter.BUTTON_CLEAR_SELECTION }
            });
        }

        [HarmonyPatch(typeof(EnchanterManager), nameof(EnchanterManager.Handle_EnchanterBehavior))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EnchanterManager_Handle_EnchanterBehavior_Transpiler
            (IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new string[] {
                    I18nKeys.Enchanter.BUTTON_ENCHANT_ENCHANT,
                    I18nKeys.Enchanter.BUTTON_ENCHANT_REROLL,
                    I18nKeys.Enchanter.BUTTON_ENCHANT_UNABLE,
                    I18nKeys.Enchanter.STATUS_NO_ENCHANT,
                    I18nKeys.Enchanter.STATUS_UNABLE_TO_ENCHANT,
                    I18nKeys.Enchanter.BUTTON_ENCHANT_INSERT_ITEM
                }
                .Concat(I18nKeys.Enchanter.BUTTON_TRANSMUTE), allowRepeat: true
            );
        }

        [HarmonyPatch(typeof(EnchanterManager), nameof(EnchanterManager.Handle_EnchanterBehavior))]
        [HarmonyPostfix]
        public static void EnchanterManager_Handle_EnchanterBehavior_Postfix
            (EnchanterManager __instance)
        {
            if (__instance._isOpen && (bool)__instance._scriptEquipment)
            {
                var _scriptEquipment = __instance._scriptEquipment;
                bool _hasTradeItemCost = (bool)_scriptEquipment && _scriptEquipment._statModifierCost != null && (bool)_scriptEquipment._statModifierCost._scriptItem;
                if (!((int)_scriptEquipment._itemRarity >= 2 || !_scriptEquipment._statModifierTable))    // copied from decompiled source, leave it to compiler for the optimization
                {
                    //_currency
                    //Text.text = $"{_setEnchantPrice} {GameManager._current._statLogics._currencyName}";
                    int _setEnchantPrice = _scriptEquipment._vendorCost * 3; // min at 3 so plural form is confirmed
                    __instance._currencyPriceText.text = $"{_setEnchantPrice} {Localyssation.GetString(I18nKeys.Lore.CROWN_PLURAL)}";
                    if (_hasTradeItemCost)
                    {
                        //int _foundTradeQuantity = 0;
                        //for (int j = 0; j < Player._mainPlayer._pInventory._heldItems.Count; j++)
                        //{
                        //    if (Player._mainPlayer._pInventory._heldItems[j]._itemName == _scriptEquipment._statModifierCost._scriptItem._itemName)
                        //    {
                        //        _foundTradeQuantity += Player._mainPlayer._pInventory._heldItems[j]._quantity;
                        //    }
                        //}
                        int _foundTradeQuantity = Player._mainPlayer._pInventory._heldItems
                            .Where(item => item._itemName == _scriptEquipment._statModifierCost._scriptItem._itemName)
                            .Aggregate(0, (sum, itemdata) => sum + itemdata._quantity);

                        __instance._tradeItemPriceText.text = $"{_foundTradeQuantity}/{_scriptEquipment._statModifierCost._scriptItemQuantity} "
                            + Localyssation.GetString(KeyUtil.GetForAsset(_scriptEquipment._statModifierCost._scriptItem) + "_NAME");
                    }
                }
                if (__instance._setItemData._modifierID > 0)
                {
                    ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(__instance._setItemData._modifierID);
                    __instance._currentEnchantmentText.text = Localyssation.GetString(I18nKeys.Enchanter.STATUS_CURRENT_ENCHANTMENT)
                        + Localyssation.GetString(KeyUtil.GetForAsset(scriptableStatModifier));
                }
            }
        }

    }

    [HarmonyPatch]
    public class EnchantmentMessagePatch
    {
        static MethodBase TargetMethod()
        {
            // 替换为实际的目标方法
            return AccessTools.Method(typeof(EnchanterManager), nameof(EnchanterManager.Init_PurchaseEnchant));
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            // 查找目标序列
            matcher.MatchForward(false, // 从当前位置向前搜索
                new CodeMatch(OpCodes.Ldstr, "You got the "),
                new CodeMatch(OpCodes.Ldloc_2), // 匹配任意ldloc指令
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableStatModifier), nameof(ScriptableStatModifier._modifierTag))),
                new CodeMatch(OpCodes.Ldstr, " enchantment!"),
                TranspilerHelper.STRING_CONCAT
            //new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ChatBehaviour), "Client_RecieveTriggerMessage", new[] { typeof(string) } ))
            );

            // 如果没找到匹配序列，返回原始指令
            if (matcher.IsInvalid)
            {
                Localyssation.logger.LogError("未找到目标IL序列，注入失败");
                return instructions;
            }


            // 创建新的指令列表来替换
            var newInstructions = new List<CodeInstruction>
            {
                // 加载局部变量（ScriptableStatModifier实例）
                new CodeInstruction(OpCodes.Ldloc_2),
            
                // 调用自定义函数
                new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(EnchantmentMessagePatch),
                    "GetCustomEnchantmentMessage"))
            };


            // 使用CodeMatcher替换指令
            matcher
                .RemoveInstructions(5) // 移除5条旧指令
                .Insert(newInstructions); // 插入新指令

            Localyssation.logger.LogDebug("成功注入自定义装备消息转换");

            return matcher.InstructionEnumeration();
        }

        // 自定义消息生成函数
        public static string GetCustomEnchantmentMessage(ScriptableStatModifier modifier)
        {
            return Localyssation.Format(I18nKeys.Enchanter.GET_NEW_ENCHANTMENT_FORMAT, KeyUtil.GetForAsset(modifier).Localize());
        }
    }

    [HarmonyPatch]
    class EnchanterTransmuteMessage
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(EnchanterManager), nameof(EnchanterManager.Init_StoneEnchant));
        }

        private static CodeMatch[] GenerateCodeMatchForTransmuteMessage(DamageType type)
        {
            return new CodeMatch[]
                {
                    new CodeMatch(OpCodes.Ldstr, "Your "),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(EnchanterManager), nameof(EnchanterManager._scriptEquipment))),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableItem), nameof(ScriptableItem._itemName))),
                    new CodeMatch(OpCodes.Ldstr, $" now scale off {type}!"),
                    TranspilerHelper.STRING_CONCAT
                };
        }

        private static CodeInstruction[] GenerateCodeInstructionsForTransmuteMessage(DamageType type)
        {
            return new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(EnchanterManager), nameof(EnchanterManager._scriptEquipment))),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EnchanterTransmuteMessage), $"GetTransmuteMessage{type}"))
                };
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceInstructions(GenerateCodeMatchForTransmuteMessage(DamageType.Strength), GenerateCodeInstructionsForTransmuteMessage(DamageType.Strength))
                .ReplaceInstructions(GenerateCodeMatchForTransmuteMessage(DamageType.Dexterity), GenerateCodeInstructionsForTransmuteMessage(DamageType.Dexterity))
                .ReplaceInstructions(GenerateCodeMatchForTransmuteMessage(DamageType.Mind), GenerateCodeInstructionsForTransmuteMessage(DamageType.Mind))
                .ReplaceStrings(new[] {
                    I18nKeys.Enchanter.NOT_ENOUGH_TRANSMUTE_STONES_DEXTERITY,
                    I18nKeys.Enchanter.NOT_ENOUGH_TRANSMUTE_STONES_MIND,
                    I18nKeys.Enchanter.NOT_ENOUGH_TRANSMUTE_STONES_STRENGTH,
                    I18nKeys.Enchanter.CANNOT_TRANSMUTE_WEAPON
                })
                .Unwrap();
        }

        public static string GetTransmuteMessageStrength(ScriptableItem item)
        {
            return Localyssation.Format(I18nKeys.Enchanter.TRANSMUTE_TO_STRENGTH_FORMAT, KeyUtil.GetForAsset(item).Name);
        }

        public static string GetTransmuteMessageDexterity(ScriptableItem item)
        {
            return Localyssation.Format(I18nKeys.Enchanter.TRANSMUTE_TO_DEXTERITY_FORMAT, KeyUtil.GetForAsset(item).Name);
        }

        public static string GetTransmuteMessageMind(ScriptableItem item)
        {
            return Localyssation.Format(I18nKeys.Enchanter.TRANSMUTE_TO_MIND_FORMAT, KeyUtil.GetForAsset(item).Name);
        }
    }
}
