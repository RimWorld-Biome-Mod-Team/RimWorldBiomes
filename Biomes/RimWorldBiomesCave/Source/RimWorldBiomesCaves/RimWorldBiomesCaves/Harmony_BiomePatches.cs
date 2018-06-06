using System;
using Harmony;
using RimWorld;
using Verse;
using Verse.Noise;
using RimWorld.BaseGen;
using UnityEngine;
using System.Linq;

namespace RimWorldBiomesCaves
{
    [StaticConstructorOnStartup]
    static class Harmony_BiomePatches
    {
        
        static Harmony_BiomePatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.swenzi.cavebiomepatches");
            harmony.Patch(AccessTools.Method(typeof(GenStep_Caves), "Generate"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(Generate_PreFix)), null);
            //harmony.Patch(AccessTools.Method(typeof(WildSpawner), "WildSpawnerTick"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(WildSpawnerTick_PostFix)));
            //harmony.Patch(AccessTools.Method(typeof(GenPlantReproduction), "TryFindReproductionDestination"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(TryFindReproductionDestination_PostFix)));
        }

        public static void WildSpawnerTick_PostFix(WildSpawner __instance){
            IntVec3 loc;
            if(Find.TickManager.TicksGame % 1210 == 0 && !__instance.AnimalEcosystemFull){
                Log.Error("not full");
                float desiredAnimalDensity = Traverse.Create(__instance).Property("DesiredAnimalDensity").GetValue<float>();
                Log.Error(desiredAnimalDensity.ToString());
                Log.Error((Rand.Value < 0.0268888883f * desiredAnimalDensity).ToString());
                Map map = (Map)AccessTools.Field(typeof(WildSpawner), "map").GetValue(__instance);
                Log.Error(RCellFinder.TryFindRandomPawnEntryCell(out loc, map, CellFinder.EdgeRoadChance_Animal, null).ToString());

            }
        }
        public static void TryFindReproductionDestination_PostFix(IntVec3 source, ThingDef plantDef, SeedTargFindMode mode, Map map, ref IntVec3 foundCell, ref bool __result)
        {
            if (plantDef.plant.cavePlant)
            {
                float radius = -1f;
                if (mode == SeedTargFindMode.Reproduce)
                {
                    radius = plantDef.plant.reproduceRadius;
                }
                else if (mode == SeedTargFindMode.MapGenCluster)
                {
                    radius = plantDef.plant.WildClusterRadiusActual;
                }
                else if (mode == SeedTargFindMode.MapEdge)
                {
                    radius = 40f;
                }
                else if (mode == SeedTargFindMode.Cave)
                {
                    radius = plantDef.plant.WildClusterRadiusActual;
                }
                int num = 0;
                int num2 = 0;
                float num3 = 0f;
                CellRect cellRect = CellRect.CenteredOn(source, Mathf.RoundToInt(radius));
                cellRect.ClipInsideMap(map);
                for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
                {
                    for (int j = cellRect.minX; j <= cellRect.maxX; j++)
                    {
                        IntVec3 c2 = new IntVec3(j, 0, i);
                        Plant plant = c2.GetPlant(map);
                        if (plant != null && (mode != SeedTargFindMode.Cave || plant.def.plant.cavePlant))
                        {
                            num++;
                            if (plant.def == plantDef)
                            {
                                num2++;
                            }
                        }
                        num3 += c2.GetTerrain(map).fertility;
                    }
                }
                float num4 = (mode != SeedTargFindMode.Cave) ? map.Biome.plantDensity : 0.5f;
                float num5 = num3 * num4;
                bool flag = (float)num > num5;
                bool flag2 = (float)num > num5 * 1.25f;
                if (flag2 && map.Biome.defName != "RWBCavern")
                {
                    foundCell = IntVec3.Invalid;
                    return;
                }
                if (mode != SeedTargFindMode.MapGenCluster && mode != SeedTargFindMode.Cave)
                {
                    BiomeDef curBiome = map.Biome;
                    float num6 = curBiome.AllWildPlants.Sum((ThingDef pd) => curBiome.CommonalityOfPlant(pd));
                    float num7 = curBiome.CommonalityOfPlant(plantDef) / num6;
                    float num8 = curBiome.CommonalityOfPlant(plantDef) * plantDef.plant.wildCommonalityMaxFraction / num6;
                    float num9 = num5 * num8;
                    if ((float)num2 > num9)
                    {
                        foundCell = IntVec3.Invalid;
                        return;
                    }
                    float num10 = num5 * num7;
                    bool flag3 = (float)num2 < num10 * 0.5f;
                    if (flag && !flag3)
                    {
                        foundCell = IntVec3.Invalid;
                        return;
                    }
                }
                Predicate<IntVec3> validator = (IntVec3 c) => plantDef.CanEverPlantAt(c, map) && (!plantDef.plant.cavePlant || GenPlantReproduction.GoodRoofForCavePlantReproduction(c, map)) && GenPlant.SnowAllowsPlanting(c, map) && source.InHorDistOf(c, radius) && GenSight.LineOfSight(source, c, map, true, null, 0, 0);
                __result = CellFinder.TryFindRandomCellNear(source, map, Mathf.CeilToInt(radius), validator, out foundCell);
                return;
            }
        }


        public static bool Generate_PreFix(Map map, GenStep_Caves __instance)
        {
            Traverse traverseobj = Traverse.Create(__instance);
            ModuleBase directionNoise = new Perlin(0.0020500000100582838, 2.0, 0.5, 4, Rand.Int, QualityMode.Medium);
            //Log.Error("patching");
            if (map.Biome.defName == "RWBCavern")
            {
                GenStep_Cavern test = new GenStep_Cavern();
                test.Generate(map);
                return false;
            }
            return true;
        }


    }
}
