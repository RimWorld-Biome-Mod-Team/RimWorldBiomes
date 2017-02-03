using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ABC_Suit
{
	public class _MapCondition_ToxicFallout : MapCondition
	{
		public override void MapConditionTick()
		{
			if (Find.TickManager.TicksGame % 3451 == 0)
			{
				List<Pawn> allPawnsSpawned = base.Map.mapPawns.AllPawnsSpawned;
				for (int i = 0; i < allPawnsSpawned.Count; i++)
				{
					Pawn pawn = allPawnsSpawned[i];
					if (!pawn.Position.Roofed(base.Map) && pawn.def.race.IsFlesh)
					{
						float num = 0f;
						if (pawn.def.race.Humanlike)
						{
							for (int j = 0; j < pawn.apparel.WornApparelCount; j++)
							{
								if (pawn.apparel.WornApparel[j].def.equippedStatOffsets != null)
								{
									for (int k = 0; k < pawn.apparel.WornApparel[j].def.equippedStatOffsets.Count<StatModifier>(); k++)
									{
										if (pawn.apparel.WornApparel[j].def.equippedStatOffsets[k].stat.ToString() == "ArmorRating_Toxin")
										{
											num += pawn.apparel.WornApparel[j].def.equippedStatOffsets.GetStatOffsetFromList(pawn.apparel.WornApparel[j].def.equippedStatOffsets[k].stat);
										}
									}
								}
							}
						}
						float num2 = 0.028758334f;
						Rand.PushSeed();
						Rand.Seed = pawn.thingIDNumber * 74374237;
						float num3 = Mathf.Lerp(0.85f, 1.15f, Rand.Value);
						Rand.PopSeed();
						num2 *= num3;
						num2 -= num2 * num;
						HealthUtility.AdjustSeverity(pawn, HediffDefOf.ToxicBuildup, num2);
					}
				}
			}
		}
	}
}
