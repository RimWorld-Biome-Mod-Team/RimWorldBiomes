using System;
using Harmony;
using RimWorld;
using Verse;
using Verse.Noise;
using RimWorld.BaseGen;
namespace RimWorldBiomesCaves
{
    static class Harmony_BiomePatches
    {
        
        static Harmony_BiomePatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.swenzi.cavebiomepatches");
            harmony.Patch(AccessTools.Method(typeof(GenStep_Caves), "Generate"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(Generate_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(RoofCollapseUtility), "WithinRangeOfRoofHolder"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(WithinRangeOfRoofHolder_PostFix)));
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

        public static void WithinRangeOfRoofHolder_PostFix(IntVec3 c, Map map, bool __result){
            if(map.roofGrid.RoofAt(c) == CavernRoofDefOf.UncollapsableNaturalRoof){
                __result = true;
            }
        }
    }
}
