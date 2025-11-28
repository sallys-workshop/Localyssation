using HarmonyLib;
using System.Collections.Generic;
using System.Text;

namespace Localyssation.Util
{
	internal static class LogHelper
	{
		public static void LogInstructions(this IEnumerable<CodeInstruction> instructions, string header = "Instructions:")
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(header);
			int index = 0;
			foreach (var instr in instructions)
			{
				sb.AppendLine($"{index}: {instr}");
				index++;
			}
			Localyssation.logger.LogInfo(sb.ToString());
		}
	}
}
