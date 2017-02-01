using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace AlienRace
{
	public static class Detours
	{
		private static List<string> detoured = new List<string>();

		private static List<string> destinations = new List<string>();

		public unsafe static bool TryDetourFromTo(MethodInfo source, MethodInfo destination)
		{
			bool flag = source == null;
			bool result;
			if (flag)
			{
				Log.Message("Source MethodInfo is null");
				result = false;
			}
			else
			{
				bool flag2 = destination == null;
				if (flag2)
				{
					Log.Message("Destination MethodInfo is null");
					result = false;
				}
				else
				{
					string text = string.Concat(new string[]
					{
						source.DeclaringType.FullName,
						".",
						source.Name,
						" @ 0x",
						source.MethodHandle.GetFunctionPointer().ToString("X" + (IntPtr.Size * 2).ToString())
					});
					string item = string.Concat(new string[]
					{
						destination.DeclaringType.FullName,
						".",
						destination.Name,
						" @ 0x",
						destination.MethodHandle.GetFunctionPointer().ToString("X" + (IntPtr.Size * 2).ToString())
					});
					bool flag3 = Detours.detoured.Contains(text);
					if (flag3)
					{
						Log.Warning(string.Concat(new string[]
						{
							"Source method ('",
							text,
							"') is previously detoured to '",
							Detours.destinations[Detours.detoured.IndexOf(text)],
							"'"
						}));
					}
					Detours.detoured.Add(text);
					Detours.destinations.Add(item);
					bool flag4 = IntPtr.Size == 8;
					if (flag4)
					{
						long num = source.MethodHandle.GetFunctionPointer().ToInt64();
						long num2 = destination.MethodHandle.GetFunctionPointer().ToInt64();
						byte* ptr = num;
						long* ptr2 = (long*)(ptr + 2);
						*ptr = 72;
						ptr[1] = 184;
						*ptr2 = num2;
						ptr[10] = 255;
						ptr[11] = 224;
					}
					else
					{
						int num3 = source.MethodHandle.GetFunctionPointer().ToInt32();
						int num4 = destination.MethodHandle.GetFunctionPointer().ToInt32();
						byte* ptr3 = num3;
						int* ptr4 = (int*)(ptr3 + 1);
						int num5 = num4 - num3 - 5;
						*ptr3 = 233;
						*ptr4 = num5;
					}
					result = true;
				}
			}
			return result;
		}
	}
}
