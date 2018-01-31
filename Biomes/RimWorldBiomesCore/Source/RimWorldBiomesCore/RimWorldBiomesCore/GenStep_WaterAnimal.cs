using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using System.Reflection;
using UnityEngine;
using System.Reflection.Emit;
using RimWorld.Planet;
using System.Runtime.CompilerServices;
using RimWorld.BaseGen;
using System.Text;
using System;
using Verse.Sound;
using Verse.AI.Group;
using Verse.Noise;
namespace RimWorldBiomesCore
{
    public class GenStep_WaterAnimal : GenStep
    {
        public override void Generate(Map map){
            //Log.Error("running");
			List<PawnKindDef> source = (from x in DefDatabase<PawnKindDef>.AllDefsListForReading
									 where x.race.GetCompProperties<RimWorldBiomesCore.CompProperties_WaterAnimal>() != null
									 && x.race.GetCompProperties<RimWorldBiomesCore.CompProperties_WaterAnimal>().allowedBiomes.Contains(map.Biome.defName) select x).ToList<PawnKindDef>();
            //Log.Error("1");
            if(source == null || source.Count == 0){
                return;
            }
            //Log.Error(source[0].defName);
			foreach (IntVec3 c in map.AllCells)
			{
				PawnKindDef source2 = source[Rand.RangeInclusive(0, source.Count - 1)];
                if (c.GetEdifice(map) == null && c.GetCover(map) == null && c.GetFirstBuilding(map) == null){
                    if(source2.race.GetCompProperties<RimWorldBiomesCore.CompProperties_WaterAnimal>().allowedTiles.Contains(c.GetTerrain(map))){
                        if(Rand.Chance(source2.race.GetCompProperties<RimWorldBiomesCore.CompProperties_WaterAnimal>().spawnChance)){
                            int randomInRange = source2.wildSpawn_GroupSizeRange.RandomInRange;
                            int radius = Mathf.CeilToInt(Mathf.Sqrt((float)source2.wildSpawn_GroupSizeRange.max));
                            for (int i = 0; i < randomInRange; i++)
                            {
                                IntVec3 loc2 = CellFinder.RandomClosewalkCellNear(c, map, radius, null);
                                Pawn newThing = PawnGenerator.GeneratePawn(source2, null);
                                GenSpawn.Spawn(newThing, loc2, map);
                            }
                        }
                    }
				}

			}
        }

    }
}
