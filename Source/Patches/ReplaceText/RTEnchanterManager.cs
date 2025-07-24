using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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

    //[HarmonyPatch]
    //public class EnchantmentMessagePatch
    //{
    //    static MethodBase TargetMethod()
    //    {
    //        // 替换为实际的目标方法
    //        return AccessTools.Method(typeof(EnchanterManager), nameof(EnchanterManager.Init_PurchaseEnchant));
    //    }

    //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var matcher = new CodeMatcher(instructions);

    //        // 查找目标序列
    //        matcher.MatchForward(false, // 从当前位置向前搜索
    //            new CodeMatch(OpCodes.Ldstr, "You got the "),
    //            new CodeMatch(OpCodes.Ldloc), // 匹配任意ldloc指令
    //            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableStatModifier), "_modifierTag")),
    //            new CodeMatch(OpCodes.Ldstr, " enchantment!"),
    //            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), "Concat", new[] { typeof(string), typeof(string), typeof(string) }))
    //        );

    //        // 如果没找到匹配序列，返回原始指令
    //        if (matcher.IsInvalid)
    //        {
    //            Localyssation.logger.LogError("未找到目标IL序列，注入失败");
    //            return instructions;
    //        }

    //        // 记录匹配位置
    //        int matchPos = matcher.Pos;

    //        // 获取ldloc指令的操作数（即局部变量索引）
    //        var ldlocInstruction = matcher.Instructions()[matchPos + 1];
    //        object localVarIndex = GetLocalVarIndex(ldlocInstruction);

    //        // 创建新的指令列表来替换
    //        var newInstructions = new List<CodeInstruction>
    //    {
    //        // 加载局部变量（ScriptableStatModifier实例）
    //        new CodeInstruction(OpCodes.Ldloc, localVarIndex),
            
    //        // 调用自定义函数
    //        new CodeInstruction(OpCodes.Call,
    //            AccessTools.Method(typeof(EnchantmentMessagePatch),
    //            "GetCustomEnchantmentMessage"))
    //    };

    //        // 使用CodeMatcher替换指令
    //        matcher
    //            .RemoveInstructions(5) // 移除5条旧指令
    //            .Insert(newInstructions); // 插入新指令

    //        Localyssation.logger.LogDebug("成功注入自定义装备消息转换");

    //        return matcher.InstructionEnumeration();
    //    }

    //    // 辅助方法：从ldloc指令中提取局部变量索引
    //    static object GetLocalVarIndex(CodeInstruction ldlocInstruction)
    //    {
    //        // 处理不同形式的ldloc指令
    //        if (ldlocInstruction.opcode == OpCodes.Ldloc_0) return 0;
    //        if (ldlocInstruction.opcode == OpCodes.Ldloc_1) return 1;
    //        if (ldlocInstruction.opcode == OpCodes.Ldloc_2) return 2;
    //        if (ldlocInstruction.opcode == OpCodes.Ldloc_3) return 3;
    //        if (ldlocInstruction.opcode == OpCodes.Ldloc_S) return ldlocInstruction.operand;
    //        if (ldlocInstruction.opcode == OpCodes.Ldloc) return ldlocInstruction.operand;

    //        Localyssation.logger.LogError($"未知的ldloc指令: {ldlocInstruction}");
    //        return 0; // 默认使用索引0
    //    }

    //    // 自定义消息生成函数
    //    public static string GetCustomEnchantmentMessage(ScriptableStatModifier modifier)
    //    {
    //        // 这里添加您的自定义逻辑
    //        return Localyssation.GetString(KeyUtil.GetForAsset(modifier));
    //    }
    //}
}
