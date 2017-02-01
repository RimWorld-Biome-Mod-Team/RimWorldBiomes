using System;
using Verse;

namespace AlienRace
{
	public static class BackstoryDefExt
	{
		public static string UniqueSaveKey(this BackstoryDef def)
		{
			bool flag = def.saveKeyIdentifier.NullOrEmpty();
			string result;
			if (flag)
			{
				result = "CustomBackstory_" + def.defName;
			}
			else
			{
				result = def.saveKeyIdentifier + "_" + def.defName;
			}
			return result;
		}
	}
}
