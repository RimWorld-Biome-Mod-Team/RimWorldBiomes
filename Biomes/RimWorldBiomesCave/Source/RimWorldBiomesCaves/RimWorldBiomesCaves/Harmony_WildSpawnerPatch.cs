using RimWorld;
using Harmony;
using Verse;
using System.Reflection;

namespace RimWorldBiomesCaves
{
    [StaticConstructorOnStartup]
    static class Harmony_WildSpawnerPatch
    {

        static Harmony_WildSpawnerPatch()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.soggynoodle.wildspawnerpatch");

            harmony.Patch(AccessTools.Method(typeof(WildSpawner), "TrySpawnPlantFromMapEdge"), null,
                new HarmonyMethod(typeof(Harmony_WildSpawnerPatch), nameof(WildSpawner_TrySpawnPlantFromMapEdge_PostFix)), null);
            Log.Message("Biomes cavern vegetation patch sends its regards");

        }

        private static void WildSpawner_TrySpawnPlantFromMapEdge_PostFix(WildSpawner __instance)
        {
            //every 2 in game seconds at speed 1
            if ((Find.TickManager.TicksGame % 60) == 0)
            {
                float SpawnedMaturity = 0.05f;
                int SpawnRate = 1;

                //are we in the caverns
                Map map = (Map)typeof(WildSpawner).GetField("map", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
                if (map.Biome.defName == "RWBCavern")
                {
                    //at normal spawnrate of 1
                    ThingDef plantDef;
                    if (SpawnRate == 1)
                    {
                        if (!map.Biome.AllWildPlants.TryRandomElementByWeight((ThingDef def) => map.Biome.CommonalityOfPlant(def), out plantDef))
                        {
                            return;
                        }
                        // Checks wether the plantdef has a fertility value(Added for TiberiumRim users since Tiberium has 0% fertility)
                        if (plantDef.fertility != 0)
                        {
                            IntVec3 source = CellFinder.RandomCell(map);
                            if (plantDef.CanEverPlantAt(source, map))
                            {
                                //insert the seed
                                GenPlantReproduction.TryReproduceInto(source, plantDef, map);
                                if (source.GetPlant(map).def == plantDef)
                                {
                                    source.GetPlant(map).Growth = SpawnedMaturity;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}