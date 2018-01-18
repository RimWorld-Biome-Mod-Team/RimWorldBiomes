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
namespace rimworld_biomes
{
    [StaticConstructorOnStartup]
    static class Harmony_BiomePatches
    {
        static Harmony_BiomePatches()
        {
            //HarmonyInstance.DEBUG = true;
            //Harmony Object Instantialization
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.swenzi.biomepatches");
            //Patches
            //Settling on Impassable Terrain Patch
            harmony.Patch(AccessTools.Method(typeof(TileFinder), "IsValidTileForNewSettlement"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(IsValidTileForNewSettlement_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(GenStep_CaveHives), "Generate"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(GenerateHive_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(GenStep_Terrain), "TerrainFrom"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(TerrainFrom_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(GenStep_Caves), "Generate"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(Generate_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(Building_SteamGeyser), "SpawnSetup"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(GeyserSpawnSetup_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(GenStep_CavePlants), "Generate"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(GenerateCavePlant_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(Building_PlantGrower), "GetInspectString"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(GetInspectString_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn_ApparelTracker), "ApparelChanged"), new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(ApparelChanged_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(Mineable), "TrySpawnYield"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(TrySpawnYield_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(JobGiver_GetFood), "TryGiveJob"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(TryGiveJob_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(World), "Impassable"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(Impassable_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(WorldPathGrid), "CalculatedCostAt"), null, new HarmonyMethod(typeof(Harmony_BiomePatches), nameof(CalculatedCostAt_PostFix)));
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
            if(__result?.targetA.Thing != null && __result?.targetA.Thing as Corpse != null){
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
				if (c.GetEdifice(map) == null && c.GetCover(map) == null && caves[c] > 0f && c.Roofed(map) && map.fertilityGrid.FertilityAt(c) > 0f)
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
		private static HashSet<IntVec3> groupSet = new HashSet<IntVec3>();
		private static HashSet<IntVec3> groupVisited = new HashSet<IntVec3>();
        private static List<IntVec3> subGroup = new List<IntVec3>();
        public static bool Generate_PreFix(Map map, GenStep_Caves __instance){
            Traverse traverseobj = Traverse.Create(__instance);
            ModuleBase directionNoise = new Perlin(0.0020500000100582838, 2.0, 0.5, 4, Rand.Int, QualityMode.Medium);
            //Log.Error("patching");
            if (map.Biome.defName == "RWBCavern"){
                GenStep_Cavern test = new GenStep_Cavern();
                test.Generate(map);
				//test.Generate(map);
				//MapGenFloatGrid elevation = MapGenerator.Elevation;
				//BoolGrid visited = new BoolGrid(map);
				//List<IntVec3> group = new List<IntVec3>();
				//foreach (IntVec3 current in map.AllCells)
				//{
				//	if (!visited[current] && IsRock(current, elevation, map))
				//	{
				//		group.Clear();
				//		map.floodFiller.FloodFill(current, (IntVec3 x) => IsRock(x, elevation, map), delegate (IntVec3 x)
				//		{
				//			visited[x] = true;
				//			group.Add(x);
				//		}, 2147483647, false, null);
				//		Trim(group, map);

				//		RemoveSmallDisconnectedSubGroups(group, map);

				//		if (group.Count >= 300)
				//		{
				//			DoOpenTunnels(group, map, __instance);
				//			DoClosedTunnels(group, map, __instance);
				//		}
				//	}
				//}
                return false;
            }
            return true;
        }

		//private static bool IsRock(IntVec3 c, MapGenFloatGrid elevation, Map map)
		//{
		//	return c.InBounds(map) && elevation[c] > 0.7f;
		//}

		//private static void Trim(List<IntVec3> group, Map map)
		//{
		//	GenMorphology.Open(group, 6, map);
		//}


  //      private static void RemoveSmallDisconnectedSubGroups(List<IntVec3> group, Map map)
  //      {
  //          groupSet.Clear();
  //          groupSet.AddRange(group);
  //          groupVisited.Clear();
  //          for (int i = 0; i < group.Count; i++)
  //          {
  //              if (!groupVisited.Contains(group[i]) && groupSet.Contains(group[i]))
  //              {
  //                  subGroup.Clear();
  //                  map.floodFiller.FloodFill(group[i], (IntVec3 x) => groupSet.Contains(x), delegate(IntVec3 x)
  //                  {
  //                      subGroup.Add(x);
  //                      groupVisited.Add(x);
  //                  }, 2147483647, false, null);
  //                  if (subGroup.Count < 300 || (float)subGroup.Count < 0.05f * (float)group.Count)
  //                  {
  //                      for (int j = 0; j < subGroup.Count; j++)
  //                      {
  //                          groupSet.Remove(subGroup[j]);
  //                      }
  //                  }
  //              }
  //          }
  //          group.Clear();
  //          group.AddRange(groupSet);
  //      }

		//private static void DoOpenTunnels(List<IntVec3> group, Map map, GenStep_Caves __instance)
		//{
		//	int num = GenMath.RoundRandom((float)group.Count * Rand.Range(0.9f, 1.1f) * 5.8f / 10000f);
		//	num = Mathf.Min(num, 3);
		//	if (num > 0)
		//	{
		//		num = Rand.RangeInclusive(1, num);
		//	}
		//	float num2 = TunnelsWidthPerRockCount.Evaluate((float)group.Count);
		//	for (int i = 0; i < num; i++)
		//	{
		//		IntVec3 start = IntVec3.Invalid;
		//		float num3 = -1f;
		//		float dir = -1f;
		//		float num4 = -1f;
		//		for (int j = 0; j < 10; j++)
		//		{
  //                  IntVec3 intVec = (IntVec3)AccessTools.Method(typeof(GenStep_Caves), "FindRandomEdgeCellForTunnel").Invoke(__instance, new object[] { group, map });
  //                  Log.Error("1");
		//			float distToCave = (float)AccessTools.Method(typeof(GenStep_Caves), "GetDistToCave").Invoke(__instance, new object[] { intVec, group, map, 40f, false });
		//			Log.Error("2");
  //                  float num6;
  //                  float num5 = FindBestInitialDir(intVec, group, out num6);
		//			Log.Error("3");
		//			Log.Error("4");
		//			if (!start.IsValid || distToCave > num3 || (distToCave == num3 && num6 > num4))
		//			{
		//				start = intVec;
		//				num3 = distToCave;
		//				dir = num5;
		//				num4 = num6;
		//			}
		//		}
		//		float width = Rand.Range(num2 * 0.8f, num2);
  //              AccessTools.Method(typeof(GenStep_Caves), "Dig").Invoke(__instance, new object[] { start, dir, group, width, map, false, null });
		//		Log.Error("5");

		//	}
		//}

		//private static void DoClosedTunnels(List<IntVec3> group, Map map, GenStep_Caves __instance)
		//{
		//	int num = GenMath.RoundRandom((float)group.Count * Rand.Range(0.9f, 1.1f) * 2.5f / 10000f);
		//	num = Mathf.Min(num, 1);
		//	if (num > 0)
		//	{
		//		num = Rand.RangeInclusive(0, num);
		//	}
		//	float num2 = TunnelsWidthPerRockCount.Evaluate((float)group.Count);
		//	for (int i = 0; i < num; i++)
		//	{
		//		IntVec3 start = IntVec3.Invalid;
		//		float num3 = -1f;
		//		for (int j = 0; j < 7; j++)
		//		{
		//			IntVec3 intVec = group.RandomElement<IntVec3>();
		//			float distToCave = (float)AccessTools.Method(typeof(GenStep_Caves), "GetDistToCave").Invoke(__instance, new object[] { intVec, group, map, 30f, true });
		//			if (!start.IsValid || distToCave > num3)
		//			{
		//				start = intVec;
		//				num3 = distToCave;
		//			}
		//		}
		//		float width = Rand.Range(num2 * 0.8f, num2);
  //              AccessTools.Method(typeof(GenStep_Caves), "Dig").Invoke(__instance, new object[] { start, Rand.Range(0f, 360f), width, group, map, true, null });
		//	}
		//}

		//private static int GetDistToNonRock(IntVec3 from, List<IntVec3> group, IntVec3 offset, int maxDist)
		//{
		//	groupSet.Clear();
		//	groupSet.AddRange(group);
		//	for (int i = 0; i <= maxDist; i++)
		//	{
		//		IntVec3 item = from + offset * i;
		//		if (!groupSet.Contains(item))
		//		{
		//			return i;
		//		}
		//	}
		//	return maxDist;
		//}
		//private static float FindBestInitialDir(IntVec3 start, List<IntVec3> group, out float dist)
		//{
		//	float num = (float)GetDistToNonRock(start, group, IntVec3.East, 40);
		//	float num2 = (float)GetDistToNonRock(start, group, IntVec3.West, 40);
		//	float num3 = (float)GetDistToNonRock(start, group, IntVec3.South, 40);
		//	float num4 = (float)GetDistToNonRock(start, group, IntVec3.North, 40);
		//	float num5 = (float)GetDistToNonRock(start, group, IntVec3.NorthWest, 40);
		//	float num6 = (float)GetDistToNonRock(start, group, IntVec3.NorthEast, 40);
		//	float num7 = (float)GetDistToNonRock(start, group, IntVec3.SouthWest, 40);
		//	float num8 = (float)GetDistToNonRock(start, group, IntVec3.SouthEast, 40);
		//	dist = Mathf.Max(new float[]
		//	{
		//		num,
		//		num2,
		//		num3,
		//		num4,
		//		num5,
		//		num6,
		//		num7,
		//		num8
		//	});
		//	return GenMath.MaxByRandomIfEqual<float>(0f, num + num8 / 2f + num6 / 2f, 45f, num8 + num3 / 2f + num / 2f, 90f, num3 + num8 / 2f + num7 / 2f, 135f, num7 + num3 / 2f + num2 / 2f, 180f, num2 + num7 / 2f + num5 / 2f, 225f, num5 + num4 / 2f + num2 / 2f, 270f, num4 + num6 / 2f + num5 / 2f, 315f, num6 + num4 / 2f + num / 2f);
		//}
    }


}
