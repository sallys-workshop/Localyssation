using HarmonyLib;
using HarmonyLib.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using UnityEngine;

namespace Localyssation.Patches
{
    public class TargetInnerMethod
    {
        /// <summary>
        /// 局部函数的核心名称（不包含编译器添加的符号）
        /// </summary>
        public string InnerMethodName { get; set; }

        /// <summary>
        /// 父方法的名称
        /// </summary>
        public string ParentMethodName { get; set; }

        /// <summary>
        /// 是否匹配参数类型
        /// </summary>
        public bool MatchParameters { get; set; } = false;

        /// <summary>
        /// 期望的参数类型列表（按顺序）
        /// </summary>
        public List<Type> ParametersTypes { get; set; } = new List<Type>();

        /// <summary>
        /// 是否匹配返回类型
        /// </summary>
        public bool MatchReturnType { get; set; } = false;

        /// <summary>
        /// 期望的返回类型
        /// </summary>
        public Type ReturnType { get; set; }

        public Type Type { get; set; }

        /// <summary>
        /// 检查方法是否匹配所有条件
        /// </summary>
        /// <param name="method">要检查的方法</param>
        /// <returns>如果满足所有条件返回 true，否则 false</returns>
        public bool IsMatch(MethodInfo method)
        {
            // 基本名称检查
            if (!CheckNamePattern(method.Name))
                return false;

            // 返回类型检查
            if (MatchReturnType && !CheckReturnType(method))
                return false;

            // 参数类型检查
            if (MatchParameters && !CheckParameters(method))
                return false;

            return true;
        }

        /// <summary>
        /// 检查方法名是否符合模式
        /// </summary>
        private bool CheckNamePattern(string methodName)
        {
            // 必须包含父方法名和局部函数名
            bool containsParent = string.IsNullOrEmpty(ParentMethodName) ||
                                  methodName.Contains(ParentMethodName);

            bool containsInner = string.IsNullOrEmpty(InnerMethodName) ||
                                 methodName.Contains(InnerMethodName);

            // 额外检查编译器生成的标识符模式
            bool hasCompilerPattern = methodName.Contains("g__") ||
                                      methodName.Contains("|") ||
                                      methodName.Contains("_");

            return containsParent && containsInner && hasCompilerPattern;
        }

        /// <summary>
        /// 检查返回类型是否匹配
        /// </summary>
        private bool CheckReturnType(MethodInfo method)
        {
            // 仅对实际方法检查返回类型
            return method.ReturnType == ReturnType;
        }

        /// <summary>
        /// 检查参数列表是否匹配
        /// </summary>
        private bool CheckParameters(MethodInfo method)
        {
            var parameters = method.GetParameters();

            // 参数数量检查
            if (parameters.Length != ParametersTypes.Count)
                return false;

            // 参数类型顺序检查
            for (int i = 0; i < parameters.Length; i++)
            {
                // 检查类型是否匹配（允许派生类型）
                if (ParametersTypes[i] != null &&
                    !ParametersTypes[i].IsAssignableFrom(parameters[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        public MethodInfo LocateOne(BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var results = FindAll(bindingFlags);
            if (results.Count() > 1)
            {
                throw new ArgumentOutOfRangeException($"More than 1 method is found. Type={Type.Name}, InnerMethodName={InnerMethodName}, ParentMethodName={ParentMethodName}");
            }
            if (results.Count() == 0)
            {
                throw new ArgumentOutOfRangeException($"No method is found. Type={Type.Name}, InnerMethodName={InnerMethodName}, ParentMethodName={ParentMethodName}");
            }
            return results.First();
        }

        public MethodInfo LocateOneOrNull(BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var results = FindAll(bindingFlags);
            if (results.Count() > 1 || results.Count() == 0)
            {
                return null;
            }
            return results.First();
        }


        /// <summary>
        /// 在类型中查找所有匹配的方法
        /// </summary>
        public IEnumerable<MethodInfo> FindAll(BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            return Type.GetMethods(bindingFlags)
                .Where(IsMatch);
        }

        public MethodInfo GetParentMethodInfo(BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            return Type.GetMethods(bindingFlags).Where(m => m.Name == ParentMethodName).DefaultIfEmpty(null).FirstOrDefault();
        }
    }

    public static class TranspilerHelper
    {
        public static readonly CodeMatch STRING_CONCAT = new CodeMatch(instr => instr.opcode == OpCodes.Call
				&& instr.operand is MethodInfo method
				&& method.DeclaringType == typeof(string)
				&& method.Name == "Concat");
        public static MethodInfo GenerateTargetMethod(TargetInnerMethod target)
        {
            try
            {
                return target.LocateOne();
            }
            catch (Exception e)
            {
                Localyssation.logger.LogError(e.Message);
                Localyssation.logger.LogError(e.StackTrace);
            }
            return target.GetParentMethodInfo();   // Fallback to edit parent method
        }

        public static IEnumerable<CodeInstruction> MatchAndReplace(IEnumerable<CodeInstruction> instructions, CodeMatch[] matches, CodeInstruction[] replacement)
        {
			return new CodeMatcher(instructions)
                .MatchForward(false, matches)
                .RemoveInstructions(matches.Length)
                .Insert(replacement)
                .Instructions();
        }

        public static CodeMatch MatchMethodCall(MethodInfo method, OpCode? opcode = null)
        {
            return
                new CodeMatch(
                    opcode ?? (method.IsStatic ? OpCodes.Call : OpCodes.Callvirt),
                    method
                    );
        }

        public static CodeMatcher ReplaceParamsStack(CodeMatcher matcher, int length)
        {
            return matcher.Advance(-length).RemoveInstructions(length);
        }

        /// <summary>
        /// Remove method params stack, searching forward
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="method">the method you want to remove its param evaluation stack</param>
        /// <param name="_ILCodeLength">IL code length of params evaluation stack, count by you own</param>
        /// <param name="opcode">OpCode of method call if you want to forcefully match a specific opcode. If leave null, will auto calculate.</param>
        /// <returns></returns>
        public static CodeMatcher RemoveMethodCallParamsStackForward(
            CodeMatcher matcher, MethodInfo method, int _ILCodeLength, OpCode? opcode = null)
        {
            return ReplaceParamsStack(
                matcher.MatchForward(true, MatchMethodCall(method, opcode)), _ILCodeLength);
                
        }
    }

    struct ILCodeReplacement
    {
        public readonly CodeMatch[] matches;
        public readonly CodeInstruction[] replacement;

        public ILCodeReplacement(CodeMatch[] matches, CodeInstruction[] replacement)
        {
            this.matches = matches;
            this.replacement = replacement;
        }
    }

    

}
