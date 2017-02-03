using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace Detours
{
	public static class Detours
	{
		private static List<string> detoured = new List<string>();

		private static List<string> destinations = new List<string>();

		public unsafe static bool TryDetourFromTo(MethodInfo source, MethodInfo destination)
		{
			if (source == null)
			{
				Log.Error("Source MethodInfo is null: Detours");
				return false;
			}
			if (destination == null)
			{
				Log.Error("Destination MethodInfo is null: Detours");
				return false;
			}
			string item = string.Concat(new string[]
			{
				source.DeclaringType.FullName,
				".",
				source.Name,
				" @ 0x",
				source.MethodHandle.GetFunctionPointer().ToString("X" + (IntPtr.Size * 2).ToString())
			});
			string item2 = string.Concat(new string[]
			{
				destination.DeclaringType.FullName,
				".",
				destination.Name,
				" @ 0x",
				destination.MethodHandle.GetFunctionPointer().ToString("X" + (IntPtr.Size * 2).ToString())
			});
			Detours.detoured.Add(item);
			Detours.destinations.Add(item2);
			if (IntPtr.Size == 8)
			{
				byte* arg_136_0 = source.MethodHandle.GetFunctionPointer().ToInt64();
				long num = destination.MethodHandle.GetFunctionPointer().ToInt64();
				byte* ptr = arg_136_0;
				long* ptr2 = (long*)(ptr + 2);
				*ptr = 72;
				ptr[1] = 184;
				*ptr2 = num;
				ptr[10] = 255;
				ptr[11] = 224;
			}
			else
			{
				int num2 = source.MethodHandle.GetFunctionPointer().ToInt32();
				int arg_1A6_0 = destination.MethodHandle.GetFunctionPointer().ToInt32();
				byte* ptr3 = num2;
				int* ptr4 = (int*)(ptr3 + 1);
				int num3 = arg_1A6_0 - num2 - 5;
				*ptr3 = 233;
				*ptr4 = num3;
			}
			return true;
		}
	}
}
