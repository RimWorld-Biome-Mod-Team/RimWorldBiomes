using RimWorld;
using System;
using Verse;

namespace ReclaimFabric
{
	public static class Helper
	{
		public static bool IsClothes(this ThingDef def)
		{
			return !def.menuHidden && (def.thingClass == typeof(Apparel) || def.thingClass.IsSubclassOf(typeof(Apparel))) && def.costStuffCount > 0;
		}
	}
}
