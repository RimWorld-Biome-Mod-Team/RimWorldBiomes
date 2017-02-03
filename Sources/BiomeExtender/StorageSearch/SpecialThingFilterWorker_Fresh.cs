using RimWorld;
using System;
using Verse;

namespace StorageSearch
{
	public class SpecialThingFilterWorker_Fresh : SpecialThingFilterWorker
	{
		public override bool Matches(Thing t)
		{
			ThingWithComps thingWithComps = t as ThingWithComps;
			if (thingWithComps == null)
			{
				return false;
			}
			CompRottable comp = thingWithComps.GetComp<CompRottable>();
			return comp != null && comp.Stage == RotStage.Fresh;
		}

		public override bool CanEverMatch(ThingDef def)
		{
			return def.HasComp(typeof(CompRottable));
		}
	}
}
