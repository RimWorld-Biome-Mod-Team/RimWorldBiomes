using Harmony;
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
    [StaticConstructorOnStartup]
    static class Harmony_BiomePatches
    {
        static Harmony_BiomePatches()
        {
            //I'm a terrible Harmony Criminal, please ignore all the evil return false prefixes you see. Transpilers to those methods would be appreciated though! Swenzi

            //HarmonyInstance.DEBUG = true;
            //Harmony Object Instantialization
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.swenzi.biomepatches");
            //Patches
            //Settling on Impassable Terrain Patch

            harmony.Patch(AccessTools.Method(typeof(TileFinder), "IsValidTileForNewSettlement"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(IsValidTileForNewSettlement_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(GenStep_CaveHives), "Generate"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(GenerateHive_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(GenStep_Terrain), "TerrainFrom"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(TerrainFrom_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(Building_SteamGeyser), "SpawnSetup"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(GeyserSpawnSetup_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(GenStep_CavePlants), "Generate"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(GenerateCavePlant_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(Building_PlantGrower), "GetInspectString"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(GetInspectString_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn_ApparelTracker), "ApparelChanged"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(ApparelChanged_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(Mineable), "TrySpawnYield"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(TrySpawnYield_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(JobGiver_GetFood), "TryGiveJob"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(TryGiveJob_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(World), "Impassable"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(Impassable_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(WorldPathGrid), "CalculatedCostAt"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(CalculatedCostAt_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(PlantProperties), "get_IsTree"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(get_IsTree_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(GenPlant), "CanEverPlantAt"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(CanEverPlantAt_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(RoofCollapseUtility), "WithinRangeOfRoofHolder"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(WithinRangeOfRoofHolder_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(GenStep_ScatterLumpsMineable), "ScatterAt"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(ScatterAt_PreFix)), null);
        }
       
        public static bool ScatterAt_PreFix(IntVec3 c, Map map, GenStep_ScatterLumpsMineable __instance){

            ThingDef thingDef = null;
            if(__instance.forcedDefToScatter != null){
                thingDef = __instance.forcedDefToScatter;
            }
            else{
                thingDef = DefDatabase<ThingDef>.AllDefs.RandomElementByWeight(delegate (ThingDef d)
                {
                    if (d.building == null)
                    {
                        return 0f;
                    }
                    if(d.GetCompProperties<CompProperties_BiomeSpecific>() != null && !d.GetCompProperties<CompProperties_BiomeSpecific>().allowedBiomes.Contains(map.Biome.defName)){
                        return 0f;
                    }
                    return d.building.mineableScatterCommonality;
                });
            }

            List<IntVec3> recentLumpCells = (List<IntVec3>)AccessTools.Field(typeof(GenStep_ScatterLumpsMineable), "recentLumpCells").GetValue(__instance);
            int numCells = (__instance.forcedLumpSize <= 0) ? thingDef.building.mineableScatterLumpSizeRange.RandomInRange : __instance.forcedLumpSize;
            recentLumpCells.Clear();
            foreach (IntVec3 current in GridShapeMaker.IrregularLump(c, map, numCells))
            {
                GenSpawn.Spawn(thingDef, current, map);
                recentLumpCells.Add(current);
            }
            AccessTools.Field(typeof(GenStep_ScatterLumpsMineable), "recentLumpCells").SetValue(__instance,recentLumpCells);
            return false;
          
        }
        public static void WithinRangeOfRoofHolder_PostFix(IntVec3 c, Map map, bool __result)
        {
            if (map.roofGrid.RoofAt(c) == RWBDefOf.UncollapsableNaturalRoof)
            {
                __result = true;
            }
        }

        public static void CanEverPlantAt_PostFix(ThingDef plantDef, IntVec3 c, Map map, ref bool __result){
            if(__result == false && plantDef.GetCompProperties<CompProperties_WaterPlant>() != null){
                if(plantDef.GetCompProperties<CompProperties_WaterPlant>().allowedTiles.Contains(c.GetTerrain(map))){
                    __result = true;
                }
            }
            
        }

        public static void CalculatedCostAt_PostFix(int tile, bool perceivedStatic, ref int __result, float yearPercent = -1f){
            Tile tile2 = Find.WorldGrid[tile];
            if(tile2.biome.defName == "RWBCavern"){
                __result = 30000;
            }
        }

        public static void Impassable_PostFix(World __instance, ref bool __result, int tileID){
            Tile tile = __instance.grid[tileID];
            if(tile.biome.defName == "RWBCavern"){
                __result = false;
            }
        }
        public static void TryGiveJob_PostFix(JobGiver_GetFood __instance, ref Job __result, Pawn pawn){
            if(__result?.targetA.Thing != null && (__result?.targetA.Thing as Corpse != null || __result?.targetA.Thing as Pawn != null && ((Pawn)__result?.targetA.Thing).Downed)){
                return;
            }
            if(__result?.def == JobDefOf.PredatorHunt && pawn?.GetComp<CompVampire>() != null){
                __result = new Job(RWBDefOf.RWBVampireBite, __result.targetA);
            }
        }
        public static void TrySpawnYield_PostFix(Map map, float yieldChance, bool moteOnWaste, Mineable __instance){
            if(__instance.def.defName == "RWBStalagmite"){
                IntVec3 current = __instance.Position;
                String thing = "";
                if (current.GetTerrain(map).defName.Contains("Sandstone"))
                {
                    thing = "ChunkSandstone";
                }
                if (current.GetTerrain(map).defName.Contains("Marble"))
                {
                    thing = "ChunkMarble";
                }
                if (current.GetTerrain(map).defName.Contains("Slate"))
                {
                    thing = "ChunkSlate";
                }
                if (current.GetTerrain(map).defName.Contains("Granite"))
                {
                    thing = "ChunkGranite";
                }
                if (current.GetTerrain(map).defName.Contains("Limestone"))
                {
                    thing = "ChunkLimestone";
                }

                int R = Rand.RangeInclusive(0, 100);
                if (R < 50 && thing != "")
                {
                    GenSpawn.Spawn(ThingDef.Named(thing), current, map);
                }

            }
        }
        public static bool ApparelChanged_PreFix(PawnGraphicSet  __instance){
            if(__instance.pawn.story == null){
                return false;
            }
            return true;
        }
                                                           
        public static bool GenerateCavePlant_PreFix(Map map){
			map.regionAndRoomUpdater.Enabled = false;
			MapGenFloatGrid caves = MapGenerator.Caves;
			List<ThingDef> source = (from x in DefDatabase<ThingDef>.AllDefsListForReading
									 where x.category == ThingCategory.Plant && x.plant.cavePlant
									 select x).ToList<ThingDef>();
			foreach (IntVec3 c in map.AllCells.InRandomOrder(null))
			{
                if (c.GetEdifice(map) == null && c.GetCover(map) == null && (caves[c] > 0f || map.Biome.defName == "RWBCavern") && c.Roofed(map) && map.fertilityGrid.FertilityAt(c) > 0f)
				{

					IEnumerable<ThingDef> source2 = from def in source
													where def.CanEverPlantAt(c, map)
													select def;
                    if (source2.Any<ThingDef>())
                    {
                        ThingDef thingDef = source2.RandomElement<ThingDef>();
                        int randomInRange = thingDef.plant.wildClusterSizeRange.RandomInRange;
                        float chance;
                        if (Math.Abs(map.Biome.CommonalityOfPlant(thingDef)) > Double.Epsilon)
                        {
                            chance = map.Biome.CommonalityOfPlant(thingDef);
                        }
                        else
                        {
                            chance = 0.18f;
                        }
                        //if(thingDef.defName == "Agarilux"){
                        //    Log.Error(thingDef.defName);
                        //    Log.Error(chance.ToString());
                        //}

                        if (Rand.Chance(chance))
                        {
                            for (int i = 0; i < randomInRange; i++)
                            {
                                IntVec3 c2;
                                if (i == 0)
                                {
                                    c2 = c;
                                }
                                else if (!GenPlantReproduction.TryFindReproductionDestination(c, thingDef, SeedTargFindMode.MapGenCluster, map, out c2))
                                {
                                    break;
                                }
                                Plant plant = (Plant)ThingMaker.MakeThing(thingDef, null);
                                plant.Growth = Rand.Range(0.07f, 1f);
                                if (plant.def.plant.LimitedLifespan)
                                {
                                    plant.Age = Rand.Range(0, Mathf.Max(plant.def.plant.LifespanTicks - 50, 0));
                                }
                                GenSpawn.Spawn(plant, c2, map);
                            }
                        }
                    }
					
				}

			}
			map.regionAndRoomUpdater.Enabled = true;
			return false;
        }
        public static bool GenerateHive_PreFix(Map map){
            if(map.Biome.defName == "RWBCavern"){
                return false;
            }
            return true;
        }

        public static bool GeyserSpawnSetup_PreFix(Map map, bool respawningAfterLoad, Building_SteamGeyser __instance){
            //if (map.Biome.defName == "Cavern"){
            //    __instance.Graphic
            //}
            return true;
        }

        public static void GetInspectString_PostFix(Building_PlantGrower __instance, ref string __result){

            if(__result.Substring(1,1) == "G"){
                __result = __result.Substring(1, __result.Length-1);
            }

            return;
        }


		public static bool IsValidTileForNewSettlement_PreFix(int tile, StringBuilder reason, ref bool __result)
		{
            //Log.Error("test");
			Tile temp_tile = Find.WorldGrid[tile];
            //Use Original method if the tile isn't a mountain or isn't a cavern
            if (temp_tile.hilliness != Hilliness.Impassable || temp_tile.biome.defName != "RWBCavern"){
                //Log.Error((temp_tile.biome.defName == "Cavern").ToString());
                return true;
			}

			Settlement settlement = Find.WorldObjects.SettlementAt(tile);
            //Are Settlements already there?
			if (settlement != null)
			{
				if (reason != null)
				{
					if (settlement.Faction == null)
					{
						reason.Append("TileOccupied".Translate());
					}
					else if (settlement.Faction == Faction.OfPlayer)
					{
						reason.Append("YourBaseAlreadyThere".Translate());
					}
					else
					{
						reason.Append("BaseAlreadyThere".Translate(new object[]
						{
										settlement.Faction.Name
						}));
					}
				}
				__result = false;
			}
			else if (Find.WorldObjects.AnySettlementAtOrAdjacent(tile))
			{
                //Settlements Next to the tile?
				if (reason != null)
				{
					reason.Append("FactionBaseAdjacent".Translate());
				}
				__result = false;
			}
			else // Can settle on the impassable terrain
			{
				__result = true;
			}
            return false;
		}

        public static bool TerrainFrom_PreFix(Map map, IntVec3 c, ref TerrainDef __result){
                        //HarmonyInstance.DEBUG = true;
            //Log.Error("test");
            //Log.Error(map.Biome.defName);
            if (map.Biome.defName == "RWBCavern"){
                //Log.Error("Called");
                TerrainDef terrainDef2;
                for (int i = 0; i < map.Biome.terrainPatchMakers.Count; i++)
                {
                    terrainDef2 = map.Biome.terrainPatchMakers[i].TerrainAt(c, map);
                    if (terrainDef2 != null)
                    {
                        //Log.Error("Changed");
                        //Log.Error(terrainDef2.defName);
                        if (c.GetFirstBuilding(map) != null)
                        {
                            c.GetFirstBuilding(map).Destroy();
                        }
                        map.roofGrid.SetRoof(c, RoofDefOf.RoofRockThick);
                        __result = terrainDef2;
                        return false;
					}
				}
            }
            return true;
        }

        public static void get_IsTree_PostFix(PlantProperties __instance, ref bool __result)
        {
            if(__instance.harvestTag == "FungalLog"){
                __result = true;
            }
        }

		private static readonly SimpleCurve TunnelsWidthPerRockCount = new SimpleCurve
		{
			{
				new CurvePoint(100f, 2f),
				true
			},
			{
				new CurvePoint(300f, 4f),
				true
			},
			{
				new CurvePoint(3000f, 5.5f),
				true
			}
		};



    }


}
