using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ReclaimFabric
{
	public static class _Thing
	{
		internal static IEnumerable<Thing> _SmeltProducts(this Thing _this, float efficiency)
		{
			List<ThingCountClass> list = _this.CostListAdjusted();
			if (!list.NullOrEmpty<ThingCountClass>())
			{
				foreach (ThingCountClass current in list)
				{
					if (!current.thingDef.intricate)
					{
						int num = GenMath.RoundRandom((float)current.count * 0.25f);
						if (num > 0)
						{
							Thing thing = ThingMaker.MakeThing(current.thingDef, null);
							thing.stackCount = num / 2;
							yield return thing;
						}
					}
				}
				List<ThingCountClass>.Enumerator enumerator = default(List<ThingCountClass>.Enumerator);
			}
			if (!_this.def.smeltProducts.NullOrEmpty<ThingCountClass>())
			{
				foreach (ThingCountClass current2 in _this.def.smeltProducts)
				{
					Thing thing2 = ThingMaker.MakeThing(current2.thingDef, null);
					thing2.stackCount = current2.count;
					yield return thing2;
				}
				List<ThingCountClass>.Enumerator enumerator = default(List<ThingCountClass>.Enumerator);
			}
			if (_this.def.IsClothes())
			{
				float t = (float)_this.Position.GetEdifice(_this.Map).InteractionCell.GetFirstPawn(_this.Map).skills.GetSkill(SkillDefOf.Crafting).Level / 20f;
				float num2 = Mathf.Lerp(0.5f, 1.5f, t);
				float t2 = (float)_this.HitPoints / (float)_this.MaxHitPoints;
				float num3 = Mathf.Lerp(0f, 0.4f, t2);
				float num4 = (float)_this.def.costStuffCount * num3 / _this.Stuff.VolumePerUnit * num2;
				int num5 = GenMath.RoundRandom(num4 * 0.25f);
				if (num5 > 0)
				{
					do
					{
						Thing thing3 = ThingMaker.MakeThing(_this.Stuff, null);
						thing3.stackCount = Mathf.Min(thing3.def.stackLimit, num5);
						num5 -= thing3.stackCount;
						yield return thing3;
					}
					while (num5 > 0);
				}
			}
			yield break;
			yield break;
		}
	}
}
